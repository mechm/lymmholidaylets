-- =============================================
-- Author:		Matt Chambers
-- Create date: 20/09/2023
-- Description: Bookings and Dates Not Available
-- =============================================
CREATE PROCEDURE [dbo].[ICalBooking_Available_GetByPropertyID]
   @PropertyID tinyint
AS
BEGIN
   SET NOCOUNT ON;

   DECLARE @TodayDate Date = CAST(GETDATE() AS DATE)

   ;WITH cte AS (
		SELECT [Date], [BookingID], Available,
				ROW_NUMBER() OVER (ORDER BY [date]) rn1,
				ROW_NUMBER() OVER (PARTITION BY [Available], [BookingID] ORDER BY [date]) rn2
		FROM  [dbo].[Calendar] WITH (NOLOCK)
		WHERE [PropertyID] =  @PropertyID --and [Available] = 0
	)

	SELECT
		CASE WHEN b.ID IS NULL AND t.[StartDate] < @TodayDate THEN DATEADD(DAY, -2, @TodayDate) ELSE t.[StartDate] END AS [StartDate],
		t.[EndDate],
		t.[BookingID], 
		b.[Name],
		RIGHT(b.Telephone,4) LastFourDigitTelephone, 
		b.[NoOfGuests],
		CASE WHEN b.ID IS NOT NULL THEN DATEDIFF(DAY,t.[StartDate],t.[EndDate]) END AS NoOfNights,
		p.FriendlyName, 
		available
	FROM(
	SELECT MIN([date]) AS [StartDate],
		   MAX([date]) AS [EndDate],
	       [BookingID], Available
	FROM cte WITH (NOLOCK) 
	GROUP BY rn1-rn2, [BookingID], Available) t
	INNER JOIN [dbo].[Property] p WITH (NOLOCK) on p.ID = @PropertyID
	LEFT JOIN [dbo].[Booking] b WITH(NOLOCK) on t.BookingID = b.ID
	WHERE t.[EndDate] >= @TodayDate and Available = 0
	ORDER BY t.[EndDate]

END