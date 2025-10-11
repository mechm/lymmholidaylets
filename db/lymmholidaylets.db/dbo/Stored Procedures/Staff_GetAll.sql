
CREATE PROCEDURE [dbo].[Staff_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ID],[Name],[YearsExperience],[JobTitle],
		[ProfileBio],[LinkedInLink],[ImagePath],[Visible]
	FROM [dbo].[Staff] with (nolock)

END