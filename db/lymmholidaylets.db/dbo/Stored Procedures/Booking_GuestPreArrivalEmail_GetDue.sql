CREATE PROCEDURE [dbo].[Booking_GuestPreArrivalEmail_GetDue]
    @RunDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = ISNULL(@RunDate, CAST(SYSUTCDATETIME() AS DATE));
    DECLARE @Now DATETIME2 (0) = SYSUTCDATETIME();

    SELECT
        B.[ID] AS [BookingID],
        B.[SessionID] AS [BookingReference],
        B.[PropertyID],
        P.[FriendlyName] AS [PropertyName],
        B.[CheckIn],
        B.[CheckOut],
        B.[NoAdult],
        B.[NoChildren],
        B.[NoInfant],
        B.[Name],
        B.[Email],
        B.[Telephone],
        B.[PostalCode],
        B.[Country],
        B.[Total],
        S.[SendDaysBeforeCheckIn],
        DATEADD(DAY, -S.[SendDaysBeforeCheckIn], CAST(B.[CheckIn] AS DATE)) AS [ScheduledSendDate]
    FROM [dbo].[Booking] B WITH (NOLOCK)
    INNER JOIN [dbo].[Property] P WITH (NOLOCK)
        ON P.[ID] = B.[PropertyID]
    INNER JOIN [dbo].[PropertyGuestEmailSchedule] S WITH (NOLOCK)
        ON S.[PropertyID] = B.[PropertyID]
    INNER JOIN [dbo].[PropertyGuestEmailTemplate] T WITH (NOLOCK)
        ON T.[PropertyID] = B.[PropertyID]
    LEFT JOIN [dbo].[GuestEmailDispatchLog] D WITH (NOLOCK)
        ON D.[BookingID] = B.[ID]
       AND D.[EmailType] = 'PreArrivalGuest'
    WHERE S.[IsEnabled] = 1
      AND NULLIF(LTRIM(RTRIM(B.[Email])), '') IS NOT NULL
      AND NULLIF(LTRIM(RTRIM(T.[HtmlBody])), '') IS NOT NULL
      AND DATEADD(DAY, -S.[SendDaysBeforeCheckIn], CAST(B.[CheckIn] AS DATE)) <= @Today
      AND (
            D.[ID] IS NULL
            OR D.[Status] = 'Failed'
            OR (
                D.[Status] IN ('Reserved', 'Published')
                AND D.[ReservationExpiresAtUtc] IS NOT NULL
                AND D.[ReservationExpiresAtUtc] < @Now
            )
        )
    ORDER BY B.[CheckIn], B.[ID];
END
