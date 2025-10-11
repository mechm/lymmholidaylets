CREATE PROCEDURE [dbo].[Template_GetAll] 
AS 
BEGIN
	SET NOCOUNT ON;

	SELECT [TemplateId], [Description] 
	FROM   [dbo].[Template] with (nolock)

END