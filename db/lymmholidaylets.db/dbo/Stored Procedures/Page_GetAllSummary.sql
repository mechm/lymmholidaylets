CREATE PROCEDURE Page_GetAllSummary	
AS
BEGIN
	SET NOCOUNT ON;

    SELECT [PageId]
	  ,[AliasTitle]
      ,[Title]  
      ,t.[Description] as TemplateDescription
	  ,Visible
	FROM [dbo].[Page] p with (nolock)
	INNER JOIN [dbo].[Template] t with (nolock) on p.TemplateId = t.TemplateId
	
END