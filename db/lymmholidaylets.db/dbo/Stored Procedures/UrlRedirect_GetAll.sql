CREATE PROCEDURE [dbo].[UrlRedirect_GetAll]     
 AS 
 BEGIN

 	SET NOCOUNT ON; 
  
 	SELECT UrlRedirectTo, UrlFrom FROM [dbo].[UrlRedirect] with (nolock) 

END