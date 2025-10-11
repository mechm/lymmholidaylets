CREATE PROCEDURE [dbo].[Review_Summaries]
@Approved bit NULL
AS
BEGIN
	SET NOCOUNT ON;

	  SELECT [PropertyID]
	      ,P.[FriendlyName] as PropertyName
		  ,[Company]
		  ,[Position]
		  ,[Name]
		  ,[EmailAddress]
		  ,t.[Description]
		  ,[Rating]
		  ,tt.[Description] as ReviewType
		  ,[LinkToView]
          ,[DateTimeAdded]
		  ,t.[ShowOnHomepage]
	  FROM [dbo].[Review] t with (nolock)
	  LEFT JOIN [dbo].[ReviewType]  tt  with (nolock) on t.ReviewTypeId  = tt.ReviewTypeId
	  INNER JOIN [dbo].[Property] P WITH (NOLOCK) on t.PropertyID = P.ID
	  WHERE (Approved = @Approved OR @Approved IS NULL)
	  ORDER BY DateTimeAdded Desc

END