CREATE PROCEDURE Email_Enquiry_GetAll	
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
	
END