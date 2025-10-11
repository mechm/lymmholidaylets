CREATE PROCEDURE [dbo].[SiteMap_GetAll]
AS 
BEGIN
	SET NOCOUNT ON; 
	
	SELECT SiteMapId, [Url] FROM [dbo].[SiteMap] with (nolock)

END