CREATE PROC [dbo].[Template_Delete] 
    @TemplateId tinyint
AS 
BEGIN
	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[Template]
	WHERE [TemplateId] = @TemplateId

END