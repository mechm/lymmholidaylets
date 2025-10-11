-- =============================================
-- Author:		Matt Chambers
-- Create date: 24/08/2024
-- Description:	Update Calendar date by Property ID and Start End Date
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_Update_Property_Date]
	@PropertyID tinyint,
	@StartDate date,
	@EndDate date
AS
BEGIN

	SET NOCOUNT ON;

    UPDATE [dbo].[Calendar]
	SET Available = 0
    WHERE PropertyID = @PropertyID
	AND [Date] >= @StartDate AND [Date] < @EndDate

END
