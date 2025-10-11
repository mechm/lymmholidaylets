CREATE PROCEDURE [dbo].[Review_GetByRegistrationCode]      
    @RegistrationCode uniqueidentifier   
AS 
BEGIN

	SET NOCOUNT ON; 

	SELECT TOP 1 ReviewId, [PropertyID], Company, [Description], [Name], EmailAddress,
                 Position, [Rating], [ReviewTypeId], [LinkToView],
		         [ShowOnHomepage], [DateTimeAdded], DateTimeApproved, RegistrationCode, Approved, Created 
				 FROM [dbo].[Review] with (nolock)
				 WHERE RegistrationCode =  @RegistrationCode

END