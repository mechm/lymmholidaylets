
CREATE PROCEDURE [dbo].[Booking_Update]
	@ID int,
    @SessionID VARCHAR(100),
    @EventID VARCHAR(100),
    @PropertyID TINYINT, 
    @CheckIn DATETIME2 (0), 
    @CheckOut DATETIME2 (0), 
    @NoAdult TINYINT NULL, 
    @NoChildren TINYINT NULL, 
    @NoInfant TINYINT NULL, 
    @Name VARCHAR(255), 
    @Email NVARCHAR(100), 
    @Telephone NVARCHAR(30),
    @PostalCode VARCHAR(10), 
    @Country VARCHAR(100),
    @Total BIGINT NULL,
	@Updated datetime2(0) NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
	    BEGIN TRANSACTION

	UPDATE [dbo].[Booking]
	   SET [SessionID] = @SessionID
		  ,[EventID] = @EventID
		  ,[PropertyID] = @PropertyID
		  ,[CheckIn] = @CheckIn
		  ,[CheckOut] = @CheckOut
		  ,[NoAdult] = @NoAdult
		  ,[NoChildren] = @NoChildren
		  ,[NoInfant] =  @NoInfant
		  ,[Name] =  @Name
		  ,[Email] = @Email
		  ,[Telephone] = @Telephone
		  ,[PostalCode] = @PostalCode
		  ,[Country] = @Country
		  ,[Total] = @Total
		  ,[Updated] = @Updated
	 WHERE ID = @ID

	  UPDATE [dbo].[Calendar]
        SET [Available] = 0,
		    [Booked] = 1,
		    [BookingID] = @ID
        WHERE [Date] >= CAST(@CheckIn AS DATE) AND [Date] < CAST(@CheckOut AS DATE)
	    AND [PropertyID] = @PropertyID
	
	  UPDATE [dbo].[Calendar]
        SET [Available] = 1,
		    [Booked] = 0,
		    [BookingID] = NULL
        WHERE [Date] < CAST(@CheckIn AS DATE) OR [Date] > CAST(@CheckOut AS DATE)	    
		AND BookingID = @ID

	  COMMIT TRANSACTION;	
	    END TRY
	    BEGIN CATCH		  
		    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		    THROW; 
	    END CATCH;

END