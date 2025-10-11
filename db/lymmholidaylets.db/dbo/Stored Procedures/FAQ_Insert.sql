CREATE PROCEDURE [dbo].[FAQ_Insert] 
	@PropertyID tinyint,
	@Question varchar(2000),
	@Answer varchar(MAX),
	@Visible bit
AS 
BEGIN
	SET NOCOUNT ON; 

	INSERT INTO [dbo].[FAQ]
           ([PropertyID]
           ,[Question]
           ,[Answer]
           ,[Visible])
     VALUES
           (@PropertyID
           ,@Question
           ,@Answer
           ,@Visible)

END