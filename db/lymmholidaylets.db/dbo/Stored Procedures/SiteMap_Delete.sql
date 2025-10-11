CREATE PROC [dbo].[SiteMap_Delete] 
    @SiteMapId tinyint
AS 
BEGIN
	SET NOCOUNT ON; 
	
	DELETE FROM [dbo].[SiteMap]
	WHERE [SiteMapId] = @SiteMapId

END