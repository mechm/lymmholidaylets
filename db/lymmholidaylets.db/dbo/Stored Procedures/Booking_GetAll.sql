
CREATE PROCEDURE [dbo].[Booking_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

  SELECT [ID],[EventID],[SessionID],[PropertyID],[CheckIn],[CheckOut],[NoAdult]
      ,[NoChildren],[NoInfant],[NoOfGuests],[Name],[Email],[Telephone]
      ,[PostalCode],[Country],[Total],[Created],[Updated]
  FROM [dbo].[Booking] with (nolock)

END