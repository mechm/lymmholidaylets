CREATE PROCEDURE [dbo].[GuestEmailDispatch_UpdateStatus]
    @BookingID INT,
    @EmailType VARCHAR (50),
    @Status VARCHAR (20),
    @FailureMessage VARCHAR (1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 (0) = SYSUTCDATETIME();

    UPDATE [dbo].[GuestEmailDispatchLog]
    SET [Status] = @Status,
        [PublishedAtUtc] = CASE WHEN @Status = 'Published' THEN @Now ELSE [PublishedAtUtc] END,
        [SentAtUtc] = CASE WHEN @Status = 'Sent' THEN @Now ELSE [SentAtUtc] END,
        [FailureMessage] = CASE WHEN @Status = 'Failed' THEN @FailureMessage ELSE NULL END,
        [ReservationExpiresAtUtc] = CASE WHEN @Status IN ('Published', 'Sent', 'Failed') THEN NULL ELSE [ReservationExpiresAtUtc] END,
        [Updated] = @Now
    WHERE [BookingID] = @BookingID
      AND [EmailType] = @EmailType;

    IF @@ROWCOUNT = 0
    BEGIN
        THROW 50000, 'GuestEmailDispatchLog row was not found for the requested booking and email type.', 1;
    END
END
