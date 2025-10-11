CREATE PROCEDURE [dbo].[Review_GetById]      
    @ReviewId int  
AS 
BEGIN

	SET NOCOUNT ON; 

	SELECT ReviewId, [PropertyID], Company, [Description], [PrivateNote], [Name], EmailAddress,
                 Position, [Rating], [Cleanliness], [Accuracy], [Communication],
           [Location],[Checkin],[Facilities],[Comfort], [Value], [ReviewTypeId], [LinkToView],
		   [ShowOnHomepage], [DateTimeAdded], DateTimeApproved, RegistrationCode, Approved, [Created]
	FROM [dbo].[Review] with (nolock)
	WHERE ReviewId =  @ReviewId

END