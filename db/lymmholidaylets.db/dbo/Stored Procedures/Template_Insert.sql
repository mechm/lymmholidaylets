CREATE PROCEDURE [dbo].[Template_Insert] 
    @Description nvarchar(50)
AS 
BEGIN
	SET NOCOUNT ON; 
		
	INSERT INTO [dbo].[Template] ([Description])
	VALUES(@Description)
	
END