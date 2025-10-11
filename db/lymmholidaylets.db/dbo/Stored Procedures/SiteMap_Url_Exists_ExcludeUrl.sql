CREATE PROCEDURE [dbo].[SiteMap_Url_Exists_ExcludeUrl] 
	@Url nvarchar(256),
	@SiteMapId tinyint
AS
BEGIN
	SET NOCOUNT ON;

    SELECT count(1)
	FROM [dbo].[SiteMap] with (nolock)
	WHERE [Url] = @Url
	AND SiteMapId <> @SiteMapId

END