CREATE PROCEDURE [dbo].[Slideshow_GetAllSummary]	
AS
BEGIN
	SET NOCOUNT ON;
		
    SELECT ID, CaptionTitle, Caption, Link, SequenceOrder, Visible
	FROM [dbo].[Slideshow] with (nolock)
	ORDER BY SequenceOrder DESC

END