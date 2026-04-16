CREATE PROCEDURE [dbo].[Property_BookingConfirmationCustomer_GetByID]
    @PropertyID TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        P.[ID] AS PropertyId,
        P.[FriendlyName] AS PropertyName,
        P.[CheckInTimeAfter],
        P.[CheckOutTimeBefore],
        A.[AddressLineOne],
        A.[AddressLineTwo],
        A.[TownOrCity],
        A.[County],
        A.[Postcode],
        A.[Country],
        P.[DirectionsUrl],
        P.[ArrivalInstructions],
        PI.[ImagePath] AS HeroImagePath,
        PI.[AltText] AS HeroImageAltText
    FROM [dbo].[Property] P WITH (NOLOCK)
    LEFT JOIN [dbo].[Address] A WITH (NOLOCK) ON A.[ID] = P.[AddressId]
    OUTER APPLY (
        SELECT TOP 1
            [ImagePath],
            [AltText]
        FROM [dbo].[PropertyImage] WITH (NOLOCK)
        WHERE [PropertyId] = P.[ID]
          AND [ShowOnSite] = 1
        ORDER BY [SequenceOrder]
    ) PI
    WHERE P.[ID] = @PropertyID;

    SELECT
        [RuleText] AS [Text],
        [SequenceOrder]
    FROM [dbo].[PropertyHouseRule] WITH (NOLOCK)
    WHERE [PropertyID] = @PropertyID
    ORDER BY [SequenceOrder], [ID];

    SELECT
        [ItemText] AS [Text],
        [SequenceOrder]
    FROM [dbo].[PropertySafetyItem] WITH (NOLOCK)
    WHERE [PropertyID] = @PropertyID
    ORDER BY [SequenceOrder], [ID];

    SELECT
        [DaysBeforeCheckIn],
        [PolicyText],
        [SequenceOrder]
    FROM [dbo].[CancellationPolicy] WITH (NOLOCK)
    WHERE [PropertyID] = @PropertyID
    ORDER BY [DaysBeforeCheckIn], [SequenceOrder], [ID];
END
