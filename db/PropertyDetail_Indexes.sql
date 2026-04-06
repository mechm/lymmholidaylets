-- =============================================
-- Property Detail Endpoint - Recommended Indexes
-- =============================================
-- Purpose: Optimize performance of Property_Detail_GetByID stored procedure
-- Created: 2026-04-05
-- Impact: Expected 5-10x performance improvement for property detail queries
-- =============================================

USE [LymmHolidayLets];
GO

-- =============================================
-- Index 1: Calendar - Booked Dates Query
-- Impact: 🔥 HIGH
-- =============================================
-- Supports: SELECT [Date] FROM Calendar WHERE PropertyID = X AND Available = 0 AND Date range
-- Benefit: Filtered index reduces size by ~50%, enables index seek + sorted retrieval
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Calendar_PropertyID_Available_Date' AND object_id = OBJECT_ID('[dbo].[Calendar]'))
BEGIN
    PRINT 'Creating index: IX_Calendar_PropertyID_Available_Date';
    CREATE NONCLUSTERED INDEX IX_Calendar_PropertyID_Available_Date
    ON [dbo].[Calendar] ([PropertyID], [Available], [Date])
    WHERE [Available] = 0;
    PRINT '✅ Index created successfully';
END
ELSE
    PRINT '⚠️  Index already exists: IX_Calendar_PropertyID_Available_Date';
GO

-- =============================================
-- Index 2: FAQ - Property FAQs Query
-- Impact: 🟡 MEDIUM
-- =============================================
-- Supports: SELECT Question, Answer FROM FAQ WHERE PropertyID = X AND Visible = 1
-- Benefit: Covering index - no key lookup needed
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FAQ_PropertyID_Visible' AND object_id = OBJECT_ID('[dbo].[FAQ]'))
BEGIN
    PRINT 'Creating index: IX_FAQ_PropertyID_Visible';
    CREATE NONCLUSTERED INDEX IX_FAQ_PropertyID_Visible
    ON [dbo].[FAQ] ([PropertyID], [Visible])
    INCLUDE ([Question], [Answer]);
    PRINT '✅ Index created successfully';
END
ELSE
    PRINT '⚠️  Index already exists: IX_FAQ_PropertyID_Visible';
GO

-- =============================================
-- Index 3: Property - Staff Property Count
-- Impact: 🟡 MEDIUM
-- =============================================
-- Supports: OUTER APPLY COUNT query for NumberOfProperties
-- Benefit: Filtered index reduces size, optimizes COUNT aggregation
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Property_StaffID_ShowOnSite' AND object_id = OBJECT_ID('[dbo].[Property]'))
BEGIN
    PRINT 'Creating index: IX_Property_StaffID_ShowOnSite';
    CREATE NONCLUSTERED INDEX IX_Property_StaffID_ShowOnSite
    ON [dbo].[Property] ([StaffId], [ShowOnSite])
    WHERE [ShowOnSite] = 1;
    PRINT '✅ Index created successfully';
END
ELSE
    PRINT '⚠️  Index already exists: IX_Property_StaffID_ShowOnSite';
GO

-- =============================================
-- Index 4: Review - Property Reviews Query
-- Impact: 🔥 HIGH
-- =============================================
-- Supports: SELECT * FROM Review WHERE PropertyID = X AND Approved = 1 ORDER BY DateTimeAdded DESC
-- Benefit: Covering index with sorted key, eliminates key lookup and separate sort
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Review_PropertyID_Approved_DateAdded' AND object_id = OBJECT_ID('[dbo].[Review]'))
BEGIN
    PRINT 'Creating index: IX_Review_PropertyID_Approved_DateAdded';
    CREATE NONCLUSTERED INDEX IX_Review_PropertyID_Approved_DateAdded
    ON [dbo].[Review] ([PropertyID], [Approved], [DateTimeAdded] DESC)
    INCLUDE ([Company], [Description], [Name], [Position], [Rating], 
             [Cleanliness], [Accuracy], [Communication], [Location], 
             [Checkin], [Facilities], [Comfort], [Value], 
             [ReviewTypeId], [LinkToView]);
    PRINT '✅ Index created successfully';
END
ELSE
    PRINT '⚠️  Index already exists: IX_Review_PropertyID_Approved_DateAdded';
GO

-- =============================================
-- Verification Queries
-- =============================================
PRINT '';
PRINT '============================================='
PRINT 'Index Creation Summary'
PRINT '============================================='

-- Check all indexes were created
SELECT 
    'Calendar' AS TableName,
    'IX_Calendar_PropertyID_Available_Date' AS IndexName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Calendar_PropertyID_Available_Date' AND object_id = OBJECT_ID('[dbo].[Calendar]'))
         THEN '✅ Exists' ELSE '❌ Missing' END AS Status
UNION ALL
SELECT 
    'FAQ',
    'IX_FAQ_PropertyID_Visible',
    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FAQ_PropertyID_Visible' AND object_id = OBJECT_ID('[dbo].[FAQ]'))
         THEN '✅ Exists' ELSE '❌ Missing' END
UNION ALL
SELECT 
    'Property',
    'IX_Property_StaffID_ShowOnSite',
    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Property_StaffID_ShowOnSite' AND object_id = OBJECT_ID('[dbo].[Property]'))
         THEN '✅ Exists' ELSE '❌ Missing' END
UNION ALL
SELECT 
    'Review',
    'IX_Review_PropertyID_Approved_DateAdded',
    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Review_PropertyID_Approved_DateAdded' AND object_id = OBJECT_ID('[dbo].[Review]'))
         THEN '✅ Exists' ELSE '❌ Missing' END;

PRINT '';
PRINT '============================================='
PRINT '✅ Index deployment complete!'
PRINT '============================================='
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Test the Property_Detail_GetByID procedure';
PRINT '2. Review execution plans (should show Index Seek operations)';
PRINT '3. Monitor performance metrics';
PRINT '4. Consider enabling READ_COMMITTED_SNAPSHOT at database level';
PRINT '';
GO

-- =============================================
-- Optional: Test Execution Plan
-- =============================================
/*
-- Uncomment to test the procedure and view execution plan

SET STATISTICS IO ON;
SET STATISTICS TIME ON;

EXEC [dbo].[Property_Detail_GetByID] @PropertyID = 1;

-- Expected Results:
-- ✅ All queries should use "Index Seek" operations
-- ✅ No "Key Lookup" operations in covered queries
-- ✅ Logical reads should be minimal (< 10 per query)
-- ✅ No table scans

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
*/
