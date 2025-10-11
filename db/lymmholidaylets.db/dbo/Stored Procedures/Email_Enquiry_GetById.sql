CREATE PROCEDURE [dbo].[Email_Enquiry_GetById]	
	@EmailEnquiryId smallint
AS
BEGIN
	SET NOCOUNT ON;

    SELECT [EmailEnquiryId]
      ,[Name]
      ,[Company]
      ,[EmailAddress]
      ,[TelephoneNo]
      ,[Subject]
      ,[Message]
      ,[DateTimeOfEnquiry]
  FROM [dbo].[EmailEnquiry] with(nolock)
  WHERE EmailEnquiryId = @EmailEnquiryId
	
END