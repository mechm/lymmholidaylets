CREATE PROCEDURE [dbo].[Slideshow_GetAll]	
AS
BEGIN
	SET NOCOUNT ON;
		
    SELECT ID, ImagePath, ImagePathAlt, CaptionTitle, Caption, ShortMobileCaption, 
	Link, SequenceOrder, Visible
	FROM [dbo].[Slideshow] with (nolock)

END