

CREATE PROCEDURE [dbo].[Staff_GetById]
 @ID tinyint
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ID],[Name],[YearsExperience],[JobTitle]
      ,[ProfileBio],[LinkedInLink],[ImagePath],[Visible]
    FROM [dbo].[Staff] with (nolock)
	WHERE [ID] = @ID

END