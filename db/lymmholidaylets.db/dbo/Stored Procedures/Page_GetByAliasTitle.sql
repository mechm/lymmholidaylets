CREATE PROCEDURE [dbo].[Page_GetByAliasTitle]
	@AliasTitle varchar(255)
AS
BEGIN
	
	SELECT [AliasTitle],
	   [MetaDescription]
      ,[Title]
      ,[MainImage]
      ,[MainImageAlt]
      ,p.[Description]
      ,t.[Description] as Template
	  ,Visible
	FROM [dbo].[Page] p with (nolock)
	INNER JOIN [dbo].[Template] t with (nolock) on p.TemplateId = t.TemplateId
	WHERE [AliasTitle] = @AliasTitle

END