CREATE PROCEDURE [dbo].[Slideshow_GetById]	
	@ID tinyint
AS
BEGIN
	SET NOCOUNT ON;
		
    SELECT ID, ImagePath, ImagePathAlt, CaptionTitle, Caption, ShortMobileCaption, 
		Link, SequenceOrder, Visible
	FROM [dbo].[Slideshow] with (nolock)
	WHERE ID = @ID

END