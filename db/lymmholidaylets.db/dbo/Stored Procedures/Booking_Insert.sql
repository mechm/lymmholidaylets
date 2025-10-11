-- =============================================
-- Author:		Matt Chambers
-- Create date: 03/09/2023
-- Description:	Saves a new Booking
-- =============================================
CREATE PROCEDURE [dbo].[Booking_Insert]
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

END
GO

