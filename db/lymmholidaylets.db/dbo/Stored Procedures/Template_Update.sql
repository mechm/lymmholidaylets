CREATE PROCEDURE [dbo].[Template_Update] 
    @TemplateId tinyint,
    @Description nvarchar(50)
AS 
BEGIN
	SET NOCOUNT ON;
	
	UPDATE [dbo].[Template]
	SET    [Description] = @Description
	WHERE  [TemplateId] = @TemplateId

END