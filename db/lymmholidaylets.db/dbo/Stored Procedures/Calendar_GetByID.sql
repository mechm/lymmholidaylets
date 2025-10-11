-- =============================================
-- Author:		Matt Chambers
-- Create date: 23/03/2024
-- Description:	Calendar by id
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_GetByID]
	@ID int	
AS
BEGIN

	SET NOCOUNT ON;

   SELECT [ID],[PropertyID],[Date],[Price],[MinimumStay]
		,[MaximumStay],[Available],[Booked],[BookingID]
    FROM [dbo].[Calendar] with (nolock)
	WHERE ID = @ID 
	
END