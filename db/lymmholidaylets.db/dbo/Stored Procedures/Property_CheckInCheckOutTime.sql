
CREATE PROCEDURE [dbo].[Property_CheckInCheckOutTime] 
	@PropertyID tinyint
AS
BEGIN

	SET NOCOUNT ON;

	SELECT [CheckInTimeAfter]
		  ,[CheckOutTimeBefore]      
	  FROM [dbo].[Property] with (nolock)
	  WHERE ID = @PropertyID 


END