CREATE PROC [dbo].[SiteMap_Insert] 
    @Url nvarchar(255)
AS 
BEGIN
	SET NOCOUNT ON; 
		
	INSERT INTO [dbo].[SiteMap] ([Url])
	VALUES(@Url)	
	
END