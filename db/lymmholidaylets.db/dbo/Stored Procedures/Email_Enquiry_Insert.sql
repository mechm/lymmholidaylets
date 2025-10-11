CREATE PROCEDURE [dbo].[Email_Enquiry_Insert] 
    @Name nvarchar(100),
    @Company nvarchar(150) = NULL,
    @EmailAddress nvarchar(100) = NULL,
    @TelephoneNo nvarchar(30) = NULL,
    @Subject nvarchar(200) = NULL,
    @Message nvarchar(MAX),  
    @DateTimeOfEnquiry datetime2(0)
AS 
BEGIN
	SET NOCOUNT ON; 
	
	INSERT INTO [dbo].[EmailEnquiry] 
		([Name], [Company], [EmailAddress], [TelephoneNo], [Subject], [Message],
		 [DateTimeOfEnquiry])
	VALUES
		(@Name, @Company, @EmailAddress, @TelephoneNo, @Subject, @Message, @DateTimeOfEnquiry)

END