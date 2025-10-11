CREATE PROCEDURE [dbo].[Staff_Delete]
	@ID [tinyint]
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[Staff] 
	WHERE ID = @ID

END