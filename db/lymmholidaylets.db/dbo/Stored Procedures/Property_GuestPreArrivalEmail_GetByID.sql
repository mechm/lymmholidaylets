CREATE PROCEDURE [dbo].[Property_GuestPreArrivalEmail_GetByID]
    @PropertyID TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        P.[ID] AS [PropertyId],
        P.[FriendlyName] AS [PropertyName],
        S.[IsEnabled],
        S.[SendDaysBeforeCheckIn],
        T.[SubjectTemplate],
        T.[PreviewTextTemplate],
        T.[HtmlBody],
        P.[CheckInTimeAfter],
        P.[CheckOutTimeBefore],
        A.[AddressLineOne],
        A.[AddressLineTwo],
        A.[TownOrCity],
        A.[County],
        A.[Postcode],
        A.[Country],
        P.[DirectionsUrl],
        P.[ArrivalInstructions]
    FROM [dbo].[Property] P WITH (NOLOCK)
    INNER JOIN [dbo].[PropertyGuestEmailSchedule] S WITH (NOLOCK)
        ON S.[PropertyID] = P.[ID]
    INNER JOIN [dbo].[PropertyGuestEmailTemplate] T WITH (NOLOCK)
        ON T.[PropertyID] = P.[ID]
    LEFT JOIN [dbo].[Address] A WITH (NOLOCK)
        ON A.[ID] = P.[AddressId]
    WHERE P.[ID] = @PropertyID;
END
