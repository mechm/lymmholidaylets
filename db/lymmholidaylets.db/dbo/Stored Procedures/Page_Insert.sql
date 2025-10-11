CREATE PROCEDURE [dbo].[Page_Insert] 
    @AliasTitle varchar(255),
    @MetaDescription varchar(1000),
    @Title varchar(255),
    @MainImage varchar(255) = NULL,
    @MainImageAlt varchar(255) = NULL,
    @Description varchar(MAX),
    @TemplateId TINYINT,
	@Visible bit
AS 
BEGIN
	SET NOCOUNT ON; 

	BEGIN TRANSACTION

	INSERT INTO [dbo].[Page] ([AliasTitle], [MetaDescription], [Title],
	[MainImage], [MainImageAlt], [Description], [TemplateId], [Visible])
	VALUES(@AliasTitle, @MetaDescription, @Title, @MainImage, @MainImageAlt, @Description, @TemplateId, @Visible)
	
	INSERT INTO UrlRedirect(UrlRedirectTo, UrlFrom)
	VALUES
	('page/' + @AliasTitle, @AliasTitle)
	 
	COMMIT TRANSACTION

END