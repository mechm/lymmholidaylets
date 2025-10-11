-- =============================================
-- Author:		Matthew Chambers
-- Create date: 30/03/2024
-- Description:	Update Multiple Calendar by ID
-- =============================================
CREATE PROCEDURE Calendar_Update_Multiple_ID
	@Price decimal(5,2) NULL,
	@MinimumStay tinyint,
	@MaximumStay smallint NULL,
	@Available bit,
	@Booked bit,
	@BookingID int NULL,
	@IDTable [dbo].[IDTable] READONLY
AS
BEGIN
	
	SET NOCOUNT ON;

    UPDATE [dbo].[Calendar]
    SET [Price] = @Price
      ,[MinimumStay] = @MinimumStay
      ,[MaximumStay] = @MaximumStay
      ,[Available] = @Available
      ,[Booked] = @Booked
      ,[BookingID] = @BookingID
	WHERE ID IN (SELECT ID From @IDTable)

END