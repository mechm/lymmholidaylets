CREATE PROCEDURE [dbo].[SiteMap_Update] 
    @SiteMapId tinyint,
    @Url nvarchar(255)
AS 
BEGIN
	SET NOCOUNT ON 	 
	
	UPDATE [dbo].[SiteMap]
	SET    [Url] = @Url
	WHERE  [SiteMapId] = @SiteMapId

END