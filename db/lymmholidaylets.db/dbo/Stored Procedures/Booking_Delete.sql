
CREATE PROCEDURE [dbo].[Booking_Delete]
	@ID int
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
	BEGIN TRANSACTION

		UPDATE [dbo].[Calendar]
        SET [Available] = 1,
		    [Booked] = 0,
		    [BookingID] = NULL
        WHERE BookingID = @ID

		DELETE FROM [dbo].[Booking] 
		WHERE ID = @ID

		COMMIT TRANSACTION;	
	    END TRY
	    BEGIN CATCH		  
		    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		    THROW; 
	    END CATCH;
END