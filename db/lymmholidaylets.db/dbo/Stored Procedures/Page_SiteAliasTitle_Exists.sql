CREATE PROCEDURE [dbo].[Page_SiteAliasTitle_Exists] 
	@AliasTitle varchar(255)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT count(1)
	FROM [dbo].[Page] with (nolock)
	WHERE AliasTitle = @AliasTitle

END