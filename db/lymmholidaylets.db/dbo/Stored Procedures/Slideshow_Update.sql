CREATE PROCEDURE [dbo].[Slideshow_Update]	
   	@ID tinyint,
	@ImagePath varchar(255),
	@ImagePathAlt varchar(255),
	@CaptionTitle varchar(500) = NULL,
	@Caption varchar(500) = NULL,
	@ShortMobileCaption varchar(255) = NULL,
	@Title varchar(255) = NULL,
	@Link varchar(255) = NULL,
	@SequenceOrder INT,
	@Visible bit
AS
BEGIN
	SET NOCOUNT ON;
		
	UPDATE [dbo].[Slideshow]
		SET ImagePath = @ImagePath,
		ImagePathAlt = @ImagePathAlt,
		CaptionTitle = @CaptionTitle,
		Caption = @Caption,	
		ShortMobileCaption = @ShortMobileCaption,
		Link = @Link,
		SequenceOrder = @SequenceOrder,	
		Visible = @Visible
	WHERE ID = @ID

END