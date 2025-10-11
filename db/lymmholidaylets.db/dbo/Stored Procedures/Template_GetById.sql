CREATE PROCEDURE [dbo].[Template_GetById] 
    @TemplateId tinyint
AS 
BEGIN
	SET NOCOUNT ON;

	SELECT [TemplateId], [Description] 
	FROM   [dbo].[Template] with (nolock) 
	WHERE  [TemplateId] = @TemplateId

END