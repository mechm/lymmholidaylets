-- =============================================
-- Author:		Matt Chambers
-- Create date: 2026-04-06
-- Description:	Lightweight lookup of CalendarLastModified for a property.
--              Used by the API cache layer to detect when calendar availability
--              has changed and the cached property detail should be evicted.
-- =============================================
CREATE PROCEDURE [dbo].[Property_GetCalendarLastModified]
    @PropertyID tinyint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [CalendarLastModified]
    FROM [dbo].[Property]
    WHERE [ID] = @PropertyID

END
