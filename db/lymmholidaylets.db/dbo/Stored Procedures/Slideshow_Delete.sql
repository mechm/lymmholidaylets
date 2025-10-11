CREATE PROCEDURE [dbo].[Slideshow_Delete]	
    @ID tinyint
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE [dbo].[Slideshow]	
	WHERE ID = @ID

END