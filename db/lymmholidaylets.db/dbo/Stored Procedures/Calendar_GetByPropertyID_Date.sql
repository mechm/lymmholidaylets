-- =============================================
-- Author:		Matt Chambers
-- Create date: 24/09/2023
-- Description:	Obtains Calendar availability based on dates
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_GetByPropertyID_Date]
	@PropertyID tinyint,
	@CheckIn date,
	@CheckOut date,
	@Available bit
AS
BEGIN
	  SET NOCOUNT ON;

      SELECT c.[Date], c.[MinimumStay], c.[MaximumStay]
	  FROM [dbo].[Calendar] c WITH (nolock)	
	  WHERE c.PropertyID = @PropertyID
	  AND c.[Date] >= @CheckIn AND c.[Date] <= @CheckOut 
	  AND c.[Available] = @Available
	  ORDER BY c.[Date] 
END