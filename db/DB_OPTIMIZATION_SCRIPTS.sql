-- =============================================
-- LymmHolidayLets Database Performance Optimization Scripts
-- =============================================
-- CRITICAL: Run in DEV first, then STAGING, then PRODUCTION
-- Backup database before running these scripts
-- =============================================

SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- =============================================
-- SECTION 1: CRITICAL OPTIMIZATIONS (PRIORITY 1)
-- =============================================

-- =================================
-- 1.1: Calendar Covering Index
-- =================================
-- Current problem: Key lookups fetch Price, MinimumStay, MaximumStay, Booked, BookingID
-- Solution: Create covering index to eliminate key lookups
-- Expected improvement: 3-5x faster for Calendar_GetByPropertyID

-- Option A: Keep both indexes (safest)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_Calendar_PropertyID_Date_Covering' AND object_id = OBJECT_ID(N'dbo.Calendar'))
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Calendar_PropertyID_Date_Covering]
        ON [dbo].[Calendar] (
            [PropertyID] ASC,
            [Date] ASC
        )
        INCLUDE (
            [ID],
            [Price],
            [MinimumStay],
            [MaximumStay],
            [Available],
            [Booked],
            [BookingID]
        )
        WHERE [Date] >= CAST(GETDATE() - 400 AS DATE)
    WITH (SORT_IN_TEMPDB = ON, ONLINE = ON);
    
    PRINT N'✓ Created IDX_Calendar_PropertyID_Date_Covering (filtered index for active dates)';
END
ELSE
BEGIN
    PRINT N'✓ IDX_Calendar_PropertyID_Date_Covering already exists';
END

-- Option B: Replace old index (if you want to save space)
-- Note: Only do this after verifying IDX_PropertyID_Date_Available is not used elsewhere
/*
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_PropertyID_Date_Available' AND object_id = OBJECT_ID(N'dbo.Calendar'))
BEGIN
    DROP INDEX [IDX_PropertyID_Date_Available] ON [dbo].[Calendar];
    PRINT N'✓ Dropped old IDX_PropertyID_Date_Available';
END
*/

-- Verify index was created
SELECT 
    i.name AS IndexName,
    STRING_AGG(c.name, ', ') AS Columns,
    i.is_unique,
    i.has_filter,
    i.filter_definition
FROM sys.indexes i
LEFT JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
LEFT JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID(N'dbo.Calendar') 
  AND i.name LIKE 'IDX_Calendar%'
GROUP BY i.name, i.is_unique, i.has_filter, i.filter_definition;

GO

-- =================================
-- 1.2: Calendar Additional Indexes for Calendar_GetByPropertyID_Date & Booking_Calendar_Upsert
-- =================================
-- Problem: Range scans on PropertyID+Date need proper indexes
-- Solution: Add index supporting filtering by Available status

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_Calendar_PropertyID_Date_Available_Filtered' AND object_id = OBJECT_ID(N'dbo.Calendar'))
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Calendar_PropertyID_Date_Available_Filtered]
        ON [dbo].[Calendar] (
            [PropertyID] ASC,
            [Available] ASC,
            [Date] ASC
        )
        INCLUDE ([BookingID])
        WHERE [Date] >= CAST(GETDATE() - 7 AS DATE)
          AND ([Available] = 0 OR [Available] = 1)
    WITH (SORT_IN_TEMPDB = ON, ONLINE = ON);
    
    PRINT N'✓ Created IDX_Calendar_PropertyID_Date_Available_Filtered for booking operations';
END
ELSE
BEGIN
    PRINT N'✓ IDX_Calendar_PropertyID_Date_Available_Filtered already exists';
END

GO

-- =================================
-- 1.3: Review Table - Missing PropertyID Index
-- =================================
-- Problem: All queries on Review filter by PropertyID but no index exists
-- Solution: Create index on (PropertyID, Approved, DateTimeAdded)
-- Expected improvement: 4-5x faster for property detail queries

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_Review_PropertyID_Approved' AND object_id = OBJECT_ID(N'dbo.Review'))
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Review_PropertyID_Approved]
        ON [dbo].[Review] (
            [PropertyID] ASC,
            [Approved] ASC,
            [DateTimeAdded] DESC
        )
        INCLUDE (
            [Name],
            [Company],
            [Position],
            [Description],
            [Rating],
            [ReviewTypeId],
            [ShowOnHomepage],
            [LinkToView],
            [Cleanliness],
            [Accuracy],
            [Communication],
            [Location],
            [Checkin],
            [Facilities],
            [Comfort],
            [Value]
        )
    WITH (SORT_IN_TEMPDB = ON, ONLINE = ON);
    
    PRINT N'✓ Created IDX_Review_PropertyID_Approved';
END
ELSE
BEGIN
    PRINT N'✓ IDX_Review_PropertyID_Approved already exists';
END

GO

-- =================================
-- 1.4: FAQ Table - Missing PropertyID Index
-- =================================
-- Problem: FAQ filtered by PropertyID but no index
-- Solution: Create covering index for FAQ queries

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_FAQ_PropertyID_Visible' AND object_id = OBJECT_ID(N'dbo.FAQ'))
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_FAQ_PropertyID_Visible]
        ON [dbo].[FAQ] (
            [PropertyID] ASC,
            [Visible] ASC
        )
        INCLUDE (
            [Question],
            [Answer]
        )
    WITH (SORT_IN_TEMPDB = ON, ONLINE = ON);
    
    PRINT N'✓ Created IDX_FAQ_PropertyID_Visible';
END
ELSE
BEGIN
    PRINT N'✓ IDX_FAQ_PropertyID_Visible already exists';
END

GO

-- =============================================
-- SECTION 2: CRITICAL PROCEDURE REFACTORING
-- =============================================

-- =================================
-- 2.1: Refactor Calendar_DateRange
-- =================================
-- Critical Issue: Recursive CTE + NOT IN subquery causes massive performance problems
-- Solution: Replace with efficient numbers table + LEFT JOIN pattern
-- Expected improvement: 2-3 minutes → 20-30 seconds (6-9x faster)

CREATE OR ALTER PROCEDURE [dbo].[Calendar_DateRange]  
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @StartDate DATE = CAST(DATEADD(DAY, 1, DATEADD(MONTH, -13, EOMONTH(GETDATE(),-1))) AS DATE);
        DECLARE @EndDate DATE = CAST(DATEADD(YEAR, 2, GETDATE()) AS DATE);
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);
        DECLARE @SixMonth DATE = CAST(DATEADD(MONTH, 6, GETDATE()) AS DATE);

        TRUNCATE TABLE [dbo].[CalendarRange];

        -- Generate date range using numbers table (much faster than recursive CTE)
        -- This uses system catalog to generate 1,095 sequential numbers
        ;WITH DateRange AS (
            SELECT TOP (DATEDIFF(DAY, @StartDate, @EndDate))
                   DATEADD(DAY, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1, @StartDate) AS AllDates
            FROM sys.all_columns a
            CROSS JOIN sys.all_columns b
        )
        INSERT INTO [dbo].[CalendarRange] (CalendarDate, Available)
        SELECT AllDates, 
               CASE WHEN AllDates >= @Today AND AllDates < @SixMonth THEN 1 ELSE 0 END AS Available
        FROM DateRange
        OPTION (MAXRECURSION 0);

        BEGIN TRANSACTION;

        -- Delete old calendar records
        DELETE FROM dbo.Calendar WHERE [Date] < @StartDate;

        -- CRITICAL FIX: Use LEFT JOIN instead of NOT IN subquery
        -- NOT IN forces nested loops + full table scans
        -- LEFT JOIN with hash match is exponentially faster
        INSERT INTO Calendar (
            [PropertyID],
            [Date],
            [Price],
            [MinimumStay],
            [MaximumStay],
            [Available]
        )
        SELECT 
            p.ID,
            cr.CalendarDate,
            p.[DefaultNightlyPrice],
            p.[DefaultMinimumStay],
            p.[DefaultMaximumStay],
            cr.Available
        FROM [dbo].[CalendarRange] cr
        CROSS JOIN [dbo].[Property] p
        LEFT JOIN [dbo].[Calendar] c 
            ON c.PropertyID = p.ID 
            AND c.[Date] = cr.CalendarDate
        WHERE c.ID IS NULL;  -- Only insert where calendar record doesn't already exist

        COMMIT TRANSACTION;
        
        PRINT N'✓ Calendar_DateRange refactored successfully';
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        PRINT N'✗ Error in Calendar_DateRange: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END;
GO

-- =================================
-- 2.2: Refactor ICalBooking_Available_GetByPropertyID
-- =================================
-- Problem: Complex dual window function + multiple JOINs prevents optimization
-- Solution: Use simpler GAPS AND ISLANDS pattern with fewer joins
-- Expected improvement: 200-400ms → 30-50ms (5-8x faster)

CREATE OR ALTER PROCEDURE [dbo].[ICalBooking_Available_GetByPropertyID]
    @PropertyID tinyint
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TodayDate DATE = CAST(GETDATE() AS DATE);

    -- Use cleaner GAPS AND ISLANDS technique
    -- Subtract row number from date to create grouping number
    ;WITH cte_with_gaps AS (
        SELECT 
            [Date],
            [BookingID],
            [Available],
            [Date] - ROW_NUMBER() OVER (PARTITION BY [BookingID] ORDER BY [Date]) AS grp
        FROM [dbo].[Calendar] WITH (NOLOCK)
        WHERE [PropertyID] = @PropertyID
          AND [Available] = 0
          AND [Date] >= @TodayDate
    ),
    cte_islands AS (
        SELECT 
            MIN([Date]) AS [StartDate],
            MAX([Date]) AS [EndDate],
            [BookingID],
            [Available]
        FROM cte_with_gaps
        GROUP BY grp, [BookingID], [Available]
    )
    SELECT 
        CASE WHEN i.[StartDate] < @TodayDate THEN @TodayDate ELSE i.[StartDate] END AS [StartDate],
        i.[EndDate],
        i.[BookingID],
        b.[Name],
        ISNULL(RIGHT(b.[Telephone], 4), '') AS LastFourDigitTelephone,
        b.[NoOfGuests],
        DATEDIFF(DAY, 
            CASE WHEN i.[StartDate] < @TodayDate THEN @TodayDate ELSE i.[StartDate] END,
            i.[EndDate]) AS NoOfNights,
        (SELECT [FriendlyName] FROM [dbo].[Property] WHERE ID = @PropertyID) AS FriendlyName,
        i.[Available]
    FROM cte_islands i
    LEFT JOIN [dbo].[Booking] b WITH (NOLOCK) ON i.[BookingID] = b.ID
    ORDER BY i.[EndDate];
    
    PRINT N'✓ ICalBooking_Available_GetByPropertyID refactored successfully';
END;
GO

-- =================================
-- 2.3: Optimize Homepage_GetAll
-- =================================
-- Problem: ORDER BY NEWID() prevents parallelization and is very slow
-- Solution: Use filtered index + deterministic ordering instead of random
-- Expected improvement: 100-150ms → 10-20ms (8-15x faster)

CREATE OR ALTER PROCEDURE [dbo].[Homepage_GetAll]	
AS
BEGIN
    SET NOCOUNT ON;

    -- Get recent approved homepage reviews in deterministic order
    -- Instead of random, use DateTimeAdded DESC for consistency
    -- Can add TABLESAMPLE if random sampling is truly required
    SELECT TOP 10 
        p.[FriendlyName],
        r.[Company],
        r.[Description],
        r.[Name],
        r.[Position],
        r.[DateTimeAdded]
    FROM [dbo].[Review] r WITH (NOLOCK)
    INNER JOIN [dbo].[Property] p WITH (NOLOCK) ON r.[PropertyID] = p.ID
    WHERE r.[ShowOnHomepage] = 1 
      AND r.[Approved] = 1
    ORDER BY r.[DateTimeAdded] DESC;

    -- Slideshow query
    SELECT 
        [ImagePath],
        [ImagePathAlt],
        [CaptionTitle],
        [Caption],
        [ShortMobileCaption],
        [Link]
    FROM [dbo].[Slideshow] WITH (NOLOCK)
    WHERE [Visible] = 1
    ORDER BY [SequenceOrder];
    
    PRINT N'✓ Homepage_GetAll optimized';
END;
GO

-- Add supporting index for Homepage_GetAll
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_Review_ShowOnHomepage_Approved' AND object_id = OBJECT_ID(N'dbo.Review'))
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Review_ShowOnHomepage_Approved]
        ON [dbo].[Review] (
            [ShowOnHomepage] ASC,
            [Approved] ASC,
            [DateTimeAdded] DESC
        )
        INCLUDE (
            [PropertyID],
            [Company],
            [Description],
            [Name],
            [Position]
        )
    WITH (SORT_IN_TEMPDB = ON, ONLINE = ON);
    
    PRINT N'✓ Created IDX_Review_ShowOnHomepage_Approved for Homepage_GetAll';
END

GO

-- =============================================
-- SECTION 3: VERIFICATION QUERIES
-- =============================================

-- Check all newly created indexes
PRINT N'';
PRINT N'========================================';
PRINT N'INDEX CREATION SUMMARY';
PRINT N'========================================';
PRINT N'';

SELECT 
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc,
    i.is_unique,
    i.has_filter,
    CAST(p.page_count * 8.0 / 1024 AS NUMERIC(10,2)) AS SizeMB,
    CAST(CAST(DATEDIFF(HOUR, STATS_DATE(t.object_id, i.index_id), GETDATE()) AS FLOAT) / 24 AS NUMERIC(10,2)) AS DaysOldStats
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') p 
    ON i.object_id = p.object_id AND i.index_id = p.index_id
WHERE (
    i.name LIKE 'IDX_Calendar%' 
    OR i.name LIKE 'IDX_Review%'
    OR i.name LIKE 'IDX_FAQ%'
)
ORDER BY t.name, i.name;

PRINT N'';
PRINT N'========================================';
PRINT N'PROCEDURE REFACTORING SUMMARY';
PRINT N'========================================';
PRINT N'';

SELECT 
    name AS ProcedureName,
    CAST(DATEDIFF(HOUR, modify_date, GETDATE()) AS INT) AS ModifiedHoursAgo,
    modify_date AS LastModified
FROM sys.procedures
WHERE schema_id = SCHEMA_ID(N'dbo')
  AND (
    name IN (
        'Calendar_DateRange',
        'ICalBooking_Available_GetByPropertyID',
        'Homepage_GetAll'
    )
  )
ORDER BY modify_date DESC;

PRINT N'';
PRINT N'✓ Database optimization complete!';
PRINT N'';
PRINT N'Next steps:';
PRINT N'1. Run: EXECUTE sp_updatestats to update query statistics';
PRINT N'2. Monitor query performance with extended events';
PRINT N'3. Compare before/after execution plans';
PRINT N'4. Test booking and calendar workflows';
PRINT N'';

-- =============================================
-- SECTION 4: PERFORMANCE MONITORING QUERIES
-- =============================================

-- Run this to check index effectiveness after optimization
-- Look for high SEEKS, low LOOKUPS, high PAGE reads
PRINT N'';
PRINT N'========================================';
PRINT N'INDEX USAGE STATISTICS (Run later)';
PRINT N'========================================';
PRINT N'';
PRINT N'After running production workloads, execute:';
PRINT N'';
PRINT N'SELECT ';
PRINT N'    OBJECT_NAME(s.object_id) AS TableName,';
PRINT N'    i.name AS IndexName,';
PRINT N'    s.user_seeks,';
PRINT N'    s.user_scans,';
PRINT N'    s.user_lookups,';
PRINT N'    s.user_updates';
PRINT N'FROM sys.dm_db_index_usage_stats s';
PRINT N'INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id';
PRINT N'WHERE database_id = DB_ID()';
PRINT N'  AND OBJECT_NAME(s.object_id) IN (''Calendar'', ''Review'', ''FAQ'')';
PRINT N'ORDER BY s.user_seeks DESC;';

GO
