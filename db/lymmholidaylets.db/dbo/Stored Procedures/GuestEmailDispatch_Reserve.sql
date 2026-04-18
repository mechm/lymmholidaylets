CREATE PROCEDURE [dbo].[GuestEmailDispatch_Reserve]
    @BookingID INT,
    @EmailType VARCHAR (50),
    @ScheduledForUtc DATETIME2 (0),
    @ReservationExpiresAtUtc DATETIME2 (0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 (0) = SYSUTCDATETIME();

    IF EXISTS (
        SELECT 1
        FROM [dbo].[GuestEmailDispatchLog] WITH (UPDLOCK, HOLDLOCK)
        WHERE [BookingID] = @BookingID
          AND [EmailType] = @EmailType
          AND (
                [Status] = 'Sent'
                OR (
                    [Status] IN ('Reserved', 'Published')
                    AND [ReservationExpiresAtUtc] IS NOT NULL
                    AND [ReservationExpiresAtUtc] >= @Now
                )
              )
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS [Reserved];
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM [dbo].[GuestEmailDispatchLog] WITH (UPDLOCK, HOLDLOCK)
        WHERE [BookingID] = @BookingID
          AND [EmailType] = @EmailType
    )
    BEGIN
        UPDATE [dbo].[GuestEmailDispatchLog]
        SET [Status] = 'Reserved',
            [ScheduledForUtc] = @ScheduledForUtc,
            [ReservationExpiresAtUtc] = @ReservationExpiresAtUtc,
            [PublishedAtUtc] = NULL,
            [SentAtUtc] = NULL,
            [FailureMessage] = NULL,
            [AttemptCount] = [AttemptCount] + 1,
            [Updated] = @Now
        WHERE [BookingID] = @BookingID
          AND [EmailType] = @EmailType;
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[GuestEmailDispatchLog]
            ([BookingID], [EmailType], [Status], [ScheduledForUtc], [ReservationExpiresAtUtc], [PublishedAtUtc], [SentAtUtc], [FailureMessage], [AttemptCount], [Created], [Updated])
        VALUES
            (@BookingID, @EmailType, 'Reserved', @ScheduledForUtc, @ReservationExpiresAtUtc, NULL, NULL, NULL, 1, @Now, NULL);
    END

    SELECT CAST(1 AS BIT) AS [Reserved];
END
