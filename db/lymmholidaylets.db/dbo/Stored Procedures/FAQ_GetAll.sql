CREATE PROCEDURE [dbo].[FAQ_GetAll] 
AS 
BEGIN
	SET NOCOUNT ON; 
	
	SELECT [ID],[PropertyID],[Question],[Answer],[Visible] 
	FROM [dbo].[FAQ] with (nolock)

END