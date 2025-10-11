
CREATE PROCEDURE [dbo].[Page_SiteAliasTitle_Exists_ExcludePage] 
	@AliasTitle varchar(255),
	@PageId tinyint
AS
BEGIN
	SET NOCOUNT ON;

    SELECT count(1)
	FROM [dbo].[Page] with (nolock)
	WHERE AliasTitle = @AliasTitle
	AND PageId <> @PageId

END