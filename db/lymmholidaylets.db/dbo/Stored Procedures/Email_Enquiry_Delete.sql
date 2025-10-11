CREATE PROCEDURE [dbo].[Email_Enquiry_Delete] 
     @EmailEnquiryId smallint
 AS 
 BEGIN

 	SET NOCOUNT ON; 
  
 	DELETE FROM [dbo].[EmailEnquiry]
 	WHERE [EmailEnquiryId] = @EmailEnquiryId

END