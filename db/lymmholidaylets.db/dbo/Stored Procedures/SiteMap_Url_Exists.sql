CREATE PROCEDURE [dbo].[SiteMap_Url_Exists] 
	@Url nvarchar(256)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT count(1)
	FROM [dbo].[SiteMap] with (nolock)
	WHERE [Url] = @Url

END