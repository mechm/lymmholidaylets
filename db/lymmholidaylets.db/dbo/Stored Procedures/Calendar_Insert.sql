-- =============================================
-- Author:		Matt Chambers
-- Create date: 04/09/2023
-- Description:	Saves a new Calendar
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_Insert]  
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
   
   INSERT INTO [dbo].[Calendar]
            ([PropertyID],
            [Date],
            [Price],
            [MinimumStay],
            [MaximumStay],
            [Available],
			[Booked],
			[BookingID])
   VALUES
           (@PropertyID,
            @Date,
            @Price,
            @MinimumStay,
            @MaximumStay,
            @Available,
			@Booked,
			@BookingID)

END
GO

