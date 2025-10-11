CREATE PROCEDURE [dbo].[Staff_Update]
	@ID [tinyint],
	@Name [nvarchar](256),
	@YearsExperience [tinyint],
	@JobTitle [varchar](256),
	@ProfileBio [varchar](3000) = NULL,
	@LinkedInLink [varchar](256) = NULL,
	@ImagePath [varchar](256),
	@Visible [bit]
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Staff]
	   SET [Name] = @Name
		  ,[YearsExperience] = @YearsExperience
		  ,[JobTitle] = @JobTitle
		  ,[ProfileBio] = @ProfileBio
		  ,[LinkedInLink] = @LinkedInLink
		  ,[ImagePath] = @ImagePath
		  ,[Visible] = @Visible
	WHERE  [ID] = @ID

END