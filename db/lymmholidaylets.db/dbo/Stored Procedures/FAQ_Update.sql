CREATE PROCEDURE [dbo].[FAQ_Update] 
	@ID tinyint,
	@PropertyID tinyint,
	@Question varchar(2000),
	@Answer varchar(MAX),
	@Visible bit
AS 
BEGIN
	SET NOCOUNT ON; 

	UPDATE [dbo].[FAQ]
	   SET [PropertyID] = @PropertyID
		  ,[Question] = @Question
		  ,[Answer] = @Answer
		  ,[Visible] = @Visible
	 WHERE ID = @ID

END