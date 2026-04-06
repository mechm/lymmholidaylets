-- =============================================
-- Author:		Matt Chambers
-- Create date: 24/08/2024
-- Description:	Update Calendar date by Property ID and Start End Date
-- Updated:     2026-04-06 - Conditionally update CalendarLastModified only when availability changes
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_Update_Property_Date]
	@PropertyID tinyint,
	@StartDate date,
	@EndDate date
AS
BEGIN

    -- Only update rows that are currently available (Available = 1).
    -- If nothing changes (already blocked), @@ROWCOUNT = 0 and the
    -- CalendarLastModified timestamp is left untouched.
    UPDATE [dbo].[Calendar]
	SET Available = 0
    WHERE PropertyID = @PropertyID
	AND [Date] >= @StartDate AND [Date] < @EndDate
	AND Available = 1

    IF @@ROWCOUNT > 0
        UPDATE [dbo].[Property]
        SET CalendarLastModified = GETUTCDATE()
        WHERE ID = @PropertyID

END
