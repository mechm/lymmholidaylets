CREATE PROC [dbo].[SiteMap_GetById] 
    @SiteMapId tinyint
AS 
BEGIN
	SET NOCOUNT ON; 
	
	SELECT SiteMapId, [Url] FROM [dbo].[SiteMap] with (nolock)
	WHERE SiteMapId = @SiteMapId

END