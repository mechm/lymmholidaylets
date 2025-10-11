CREATE PROCEDURE [dbo].[FAQ_GetByID] 
	@ID tinyint
AS 
BEGIN
	SET NOCOUNT ON; 
	
	SELECT [ID],[PropertyID],[Question],[Answer],[Visible] 
	FROM [dbo].[FAQ] with (nolock)
	WHERE ID = @ID

END