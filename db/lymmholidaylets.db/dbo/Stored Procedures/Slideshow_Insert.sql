CREATE PROCEDURE [dbo].[Slideshow_Insert]
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

	INSERT INTO Slideshow(ImagePath,ImagePathAlt,CaptionTitle,Caption,ShortMobileCaption,Link,SequenceOrder,Visible)
	VALUES(@ImagePath,@ImagePathAlt,@CaptionTitle,@Caption,@ShortMobileCaption,@Link,@SequenceOrder,@Visible)
	
END