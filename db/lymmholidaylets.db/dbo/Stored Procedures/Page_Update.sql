CREATE PROCEDURE [dbo].[Page_Update] 
    @PageId tinyint,
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

	DECLARE @AliasTitleToCheck VARCHAR(255)
	SET @AliasTitleToCheck = (SELECT TOP 1 AliasTitle FROM [dbo].[Page] with (nolock) WHERE PageId = @PageId)

	UPDATE [dbo].[Page]
	SET    [AliasTitle] = @AliasTitle, 
		   [MetaDescription] = @MetaDescription,
		   [Title] = @Title, 
		   [MainImage] = @MainImage,
		   [MainImageAlt] = @MainImageAlt, 
		   [Description] = @Description,
		   [TemplateId] = @TemplateId, 
		   [Visible] = @Visible
	WHERE  [PageId] = @PageId

	MERGE [dbo].[UrlRedirect] AS target  
    USING (SELECT @AliasTitleToCheck) AS source (UrlFrom)  
    ON (target.UrlFrom = source.UrlFrom)  
    WHEN MATCHED THEN   
        UPDATE SET UrlRedirectTo = 'page/' + @AliasTitle ,
		UrlFrom = @AliasTitle
    WHEN NOT MATCHED THEN  
        INSERT (UrlRedirectTo, UrlFrom)  
        VALUES ('page/' + @AliasTitle, @AliasTitle);  

	COMMIT TRANSACTION

END