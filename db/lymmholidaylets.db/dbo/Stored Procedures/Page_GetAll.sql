CREATE PROCEDURE Page_GetAll	
AS
BEGIN
	SET NOCOUNT ON;

    SELECT [PageId],
	   [AliasTitle],
       [MetaDescription]
      ,[Title]
      ,[MainImage]
      ,[MainImageAlt]
      ,p.[Description]
	  ,t.TemplateId
      ,t.[Description] as TemplateDescription
	  ,Visible
	FROM [dbo].[Page] p with (nolock)
	INNER JOIN [dbo].[Template] t with (nolock) on p.TemplateId = t.TemplateId
	
END