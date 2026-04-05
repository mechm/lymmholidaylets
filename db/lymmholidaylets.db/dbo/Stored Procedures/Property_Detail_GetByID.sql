-- =============================================
-- Author:		Matt Chambers
-- Create date: 26/11/2023
-- Description:	Booking conditions for a property and Availability
-- Updated:     2026-04-05 - Extended to include DisplayAddress, Description, Host, and Map data
-- Optimized:   2026-04-05 - Performance improvements (removed correlated subquery, removed NOLOCK, pre-calculate dates)
-- =============================================
CREATE PROCEDURE [dbo].[Property_Detail_GetByID]
	@PropertyID tinyint
AS
BEGIN
	SET NOCOUNT ON;

    -- Pre-calculate date range once for better performance
    DECLARE @StartDate DATE = DATEADD(DAY, 1, EOMONTH(GETDATE(), -1));
    DECLARE @EndDate DATE = DATEADD(YEAR, 1, EOMONTH(GETDATE()));

    -- Property booking capacity, display info, host, and map details
    SELECT 
        P.[ID], 
        P.[MinimumNumberOfAdult], 
        P.[MaximumNumberOfAdult], 
        P.[MaximumNumberOfGuests], 
		P.[MaximumNumberOfChildren], 
        P.[MaximumNumberOfInfants],
        P.[DisplayAddress],
        P.[Description] AS PageDescription,
        -- Host information
        S.[Name] AS HostName,
        '' AS HostLocation,
        -- OPTIMIZED: Use OUTER APPLY instead of correlated subquery to avoid repeated table scans
        ISNULL(PC.PropCount, 0) AS NumberOfProperties,
        S.[YearsExperience] AS HostYearsExperience,
        S.[JobTitle] AS HostJobTitle,
        S.[ProfileBio] AS HostProfileBio,
        S.[ImagePath] AS HostImagePath,
        -- Map information
        ISNULL(P.[ShowGoogleMap], 0) AS ShowMap,
        ISNULL(P.[ShowStreetView], 0) AS ShowStreetView,
        ISNULL(G.[Latitude], 0) AS Latitude,
        ISNULL(G.[Longitude], 0) AS Longitude,
        ISNULL(G.[MapZoom], 0) AS MapZoom,
        ISNULL(G.[StreetViewLatitude], 0) AS StreetViewLatitude,
        ISNULL(G.[StreetViewLongitude], 0) AS StreetViewLongitude,
        ISNULL(G.[Pitch], 0) AS Pitch,
        ISNULL(G.[Yaw], 0) AS Yaw,
        ISNULL(G.[Zoom], 0) AS Zoom
    FROM [dbo].[Property] P
    LEFT JOIN [dbo].[Staff] S ON P.[StaffId] = S.[ID]
    LEFT JOIN [dbo].[GeoLocation] G ON P.[GeoLocationId] = G.[ID]
    -- OPTIMIZED: Replace correlated subquery with OUTER APPLY for better performance
    OUTER APPLY (
        SELECT COUNT(*) AS PropCount
        FROM [dbo].[Property] 
        WHERE [StaffId] = P.[StaffId] 
          AND [ShowOnSite] = 1
    ) PC
    WHERE P.[ID] = @PropertyID

    -- Booked dates within the next year
    -- OPTIMIZED: Use pre-calculated variables and >= <= operators for better index usage
    SELECT [Date]
    FROM [dbo].[Calendar]
    WHERE [PropertyID] = @PropertyID 
      AND [Available] = 0 
      AND [Date] >= @StartDate 
      AND [Date] <= @EndDate
    ORDER BY [Date];

	-- FAQs
	SELECT [Question], [Answer]
	FROM [dbo].[FAQ]
	WHERE [PropertyID] = @PropertyID 
	  AND [Visible] = 1;

 	-- Reviews with review type
 	SELECT 
        [Company],
        R.[Description],
        [Name],
        [Position],
        [Rating],
        [Cleanliness],
        [Accuracy],
        [Communication],
        [Location],
        [Checkin],
        [Facilities],
        [Comfort],
        [Value],
        RT.[Description] AS ReviewType,
        [LinkToView],
        [DateTimeAdded]
    FROM [dbo].[Review] R
	INNER JOIN [dbo].[ReviewType] RT ON RT.ReviewTypeId = R.ReviewTypeId
	WHERE [PropertyID] = @PropertyID 
	  AND [Approved] = 1
	ORDER BY [DateTimeAdded] DESC;

END