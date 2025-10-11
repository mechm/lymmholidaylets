-- =============================================
-- Author:		Matt Chambers
-- Create date: 04/09/2023
-- Description:	Updates a calendar
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_Update]
    @ID          INT,
    @PropertyID  TINYINT,
    @Date        DATE,
    @Price       DECIMAL (5,2) NULL,
    @MinimumStay TINYINT,
    @MaximumStay SMALLINT NULL,
    @Available   BIT,
	@Booked      BIT,
	@BookingID   INT NULL
AS
BEGIN
   SET NOCOUNT ON;

    UPDATE [dbo].[Calendar]
    SET [PropertyID] = @PropertyID,
        [Date] = @Date,
        [Price] = @Price,
        [MinimumStay] = @MinimumStay,
        [MaximumStay] = @MaximumStay,
        [Available] = @Available,
		[Booked] = @Booked,
		[BookingID] = @BookingID
    WHERE [ID] = @ID

END
GO

