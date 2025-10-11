-- =============================================
-- Author:		Matt Chambers
-- Create date: 05/10/2023
-- Description:	Booking conditions for a property
-- =============================================
CREATE PROCEDURE [dbo].[Property_Booking_GetByID]
	@PropertyID tinyint
AS
BEGIN
	SET NOCOUNT ON;

     SELECT [MaximumNumberOfGuests]
	  ,[MinimumNumberOfAdult]
      ,[MaximumNumberOfAdult]
      ,[MaximumNumberOfChildren]
      ,[MaximumNumberOfInfants]
	 FROM [dbo].[Property] WITH (nolock)
	 WHERE ID = @PropertyID 
END