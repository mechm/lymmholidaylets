CREATE PROCEDURE [dbo].[Review_GetAll]      
    
AS 
BEGIN

	SET NOCOUNT ON; 

	SELECT ReviewId, [PropertyID], Company, [Description], [PrivateNote], [Name], EmailAddress,
                 Position, [Rating], [Cleanliness], [Accuracy], [Communication],
           [Location],[Checkin],[Facilities],[Comfort], [Value], [ReviewTypeId], [LinkToView],
		   [ShowOnHomepage], [DateTimeAdded], DateTimeApproved, RegistrationCode, Approved, [Created]
	FROM [dbo].[Review] with (nolock)

END