-- =============================================
-- Author:		Matt Chambers
-- Create date: 23/03/2024
-- Description:	Calendar per property
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_GetByPropertyID]
	@PropertyID tinyint,
	@StartDate date,
	@EndDate date
AS
BEGIN

	SET NOCOUNT ON;

   SELECT [ID],[PropertyID],[Date],[Price] ,[MinimumStay]
		,[MaximumStay],[Available],[Booked],[BookingID]
    FROM [dbo].[Calendar] with (nolock)
	WHERE PropertyID = @PropertyID 
	AND [Date] BETWEEN @StartDate AND @EndDate
	ORDER BY [Date] asc
END