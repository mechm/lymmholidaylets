CREATE PROCEDURE [dbo].[Page_Delete] 
     @PageId tinyint
 AS 
 BEGIN

 	SET NOCOUNT ON; 
  	
	BEGIN TRANSACTION

	DECLARE @AliasTitle VARCHAR(255)
	SET @AliasTitle = (SELECT TOP 1 AliasTitle FROM [dbo].[Page] WHERE PageId = @PageId)

 	DELETE FROM [dbo].[Page]
 	WHERE [PageId] = @PageId

	DELETE FROM [dbo].[UrlRedirect]
	WHERE UrlFrom = @AliasTitle
	 
	COMMIT TRANSACTION

END