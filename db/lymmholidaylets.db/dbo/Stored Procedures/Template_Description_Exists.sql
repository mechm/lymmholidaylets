CREATE PROCEDURE [dbo].[Template_Description_Exists] 
	@Description nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT count(1)
	FROM [dbo].[Template] with (nolock)
	WHERE [Description] = @Description

END