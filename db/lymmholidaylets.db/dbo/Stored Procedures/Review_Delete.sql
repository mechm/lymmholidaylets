CREATE PROC [dbo].[Review_Delete] 
    @ReviewId int
AS 
BEGIN
	SET NOCOUNT ON;	
	
	DELETE FROM [dbo].[Review]
	WHERE [ReviewId] = @ReviewId

END