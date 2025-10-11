CREATE PROCEDURE Page_Exists_GetByTemplateId
@TemplateId tinyint
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT count(1) FROM [dbo].[Page] p with (nolock)
	INNER JOIN [dbo].[Template] t with (nolock) on p.TemplateId = t.TemplateId
	WHERE t.[TemplateId] = @TemplateId)
	PRINT 'true'
	ELSE
	PRINT 'false'
	
END