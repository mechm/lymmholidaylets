CREATE PROCEDURE [dbo].[Review_Insert] 
    @PropertyID tinyint,
    @Company varchar(256) = NULL,  
    @Description varchar(2000),
    @PrivateNote varchar(2000),
    @Name varchar(256),
    @EmailAddress varchar(100) = NULL,
    @Position varchar(100) = NULL,  
    @Rating tinyint,
    @Cleanliness tinyint = NULL,
    @Accuracy tinyint = NULL,
    @Communication tinyint = NULL,
    @Location tinyint = NULL,
    @Checkin tinyint = NULL,
    @Facilities tinyint = NULL,
    @Comfort tinyint = NULL,
    @Value tinyint = NULL,
    @ReviewTypeId TINYINT = NULL,
    @LinkToView VARCHAR (256) = NULL,
    @ShowOnHomepage BIT = NULL,
    @DateTimeAdded DATETIME2(0) = NULL,
    @RegistrationCode uniqueidentifier,
    @DateTimeApproved DATETIME2(0) = NULL,
    @Approved bit,
	@Created DATETIME2(0)
AS 
BEGIN

	SET NOCOUNT ON; 

	INSERT INTO [dbo].[Review] ([PropertyID], [Company], [Description], [PrivateNote], [Name], [EmailAddress], 
	[Position], [Rating],[Cleanliness],[Accuracy],[Communication],[Location],[Checkin],[Facilities],
    [Comfort],[Value],[ReviewTypeId], [LinkToView], [ShowOnHomepage], [DateTimeAdded],
    [RegistrationCode], [DateTimeApproved], [Approved], [Created])
	VALUES(@PropertyID, @Company, @Description, @PrivateNote, @Name, @EmailAddress, @Position, @Rating, @Cleanliness,
    @Accuracy, @Communication, @Location, @Checkin, @Facilities, @Comfort, @Value,
    @ReviewTypeId, @LinkToView, @ShowOnHomepage, @DateTimeAdded, @RegistrationCode,
	@DateTimeApproved, @Approved, @Created)	
	
END