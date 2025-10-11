-- =============================================
-- Author:		Matt Chambers
-- Create date: 06/09/2023
-- Description:	Saves a new Booking and Calendar Update
-- =============================================
CREATE PROCEDURE [dbo].[Booking_Calendar_Upsert]
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
    @Created datetime2(0)
AS
BEGIN	
	SET NOCOUNT ON;

	
	DECLARE @BookingID INT

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Booking] WHERE [PropertyID] = @PropertyID AND [CheckIn] = @CheckIn AND [CheckOut] = @CheckOut)
	BEGIN

    	BEGIN TRY
	    BEGIN TRANSACTION

	    INSERT INTO [dbo].[Booking]
                ([SessionID],
                [EventID],
                [PropertyID], 
                [CheckIn],
                [CheckOut],
                [NoAdult], 
                [NoChildren], 
                [NoInfant], 
                [Name],
                [Email],
                [Telephone],
                [PostalCode],
                [Country],
                [Total],
                [Created])
       VALUES
               (@SessionID,
                @EventID,
                @PropertyID, 
                @CheckIn, 
                @CheckOut, 
                @NoAdult,
                @NoChildren,
                @NoInfant,
                @Name, 
                @Email, 
                @Telephone,
                @PostalCode, 
                @Country,
                @Total,
                @Created)

        SET @BookingID = (SELECT TOP 1 [ID]
						    FROM [dbo].[Booking]
						    WHERE @@ROWCOUNT > 0 AND [ID] = scope_identity())

	    UPDATE [dbo].[Calendar]
        SET [Available] = 0,
		    [Booked] = 1,
		    [BookingID] = @BookingID
        WHERE [Date] >= CAST(@CheckIn AS DATE) AND [Date] < CAST(@CheckOut AS DATE)
	    AND [PropertyID] = @PropertyID

        COMMIT TRANSACTION;	
	    END TRY
	    BEGIN CATCH		  
		    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		    THROW; 
	    END CATCH;

    END

END