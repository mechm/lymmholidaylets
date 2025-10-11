CREATE PROCEDURE Email_Enquiry_Update	
	@EmailEnquiryId smallint,
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

   Update [dbo].[EmailEnquiry]
   SET [Name] = @Name,
       Company = @Company,
	   EmailAddress = @EmailAddress,
	   TelephoneNo = @TelephoneNo,
	   [Subject] = @Subject,
	   [Message] = @Message,
	   [DateTimeOfEnquiry] = @DateTimeOfEnquiry
   WHERE EmailEnquiryId = @EmailEnquiryId
	
END