
CREATE PROCEDURE [dbo].[Staff_Insert]
	@Name nvarchar(256),
	@YearsExperience tinyint,
	@JobTitle varchar(256),
	@ProfileBio varchar(3000) NULL,
	@LinkedInLink varchar(256) NULL,
	@ImagePath varchar(256),
	@Visible bit
AS
BEGIN
	
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Staff]
           ([Name]
           ,[YearsExperience]
           ,[JobTitle]
           ,[ProfileBio]
           ,[LinkedInLink]
           ,[ImagePath]
           ,[Visible])
     VALUES
           (@Name
           ,@YearsExperience
           ,@JobTitle
           ,@ProfileBio
           ,@LinkedInLink 
           ,@ImagePath
           ,@Visible)
END