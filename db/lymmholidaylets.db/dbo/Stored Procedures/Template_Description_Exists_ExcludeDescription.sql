CREATE PROCEDURE [dbo].[Template_Description_Exists_ExcludeDescription] 
	@Description nvarchar(50),
	@TemplateId tinyint
AS
BEGIN
	SET NOCOUNT ON;

	SELECT count(1)
	FROM [dbo].[Template] with (nolock)
	WHERE [Description] = @Description
	AND TemplateId <> @TemplateId	

END