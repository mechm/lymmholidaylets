CREATE PROCEDURE [dbo].[Property_BookingConfirmationCustomer_GetByID]
    @PropertyID TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Result set 1: Core property details for the customer booking confirmation email
    SELECT
        p.ID                AS PropertyId,
        p.FriendlyName      AS PropertyName,
        p.Bedroom,
        p.Bathroom,
        p.CheckInTimeAfter,
        p.CheckOutTimeBefore,
        a.AddressLineOne,
        a.AddressLineTwo,
        a.TownOrCity,
        a.County,
        a.Postcode,
        a.Country,
        p.DirectionsUrl,
        p.ArrivalInstructions,
        pi.ImagePath        AS HeroImagePath,
        pi.AltText          AS HeroImageAltText
    FROM [dbo].[Property] p
    LEFT JOIN [dbo].[Address] a
        ON a.ID = p.AddressId
    LEFT JOIN [dbo].[PropertyImage] pi
        ON pi.PropertyId = p.ID
        AND pi.ForEmail = 1
    WHERE p.ID = @PropertyID;

    -- Result set 2: House rules (ordered for display)
    SELECT
        RuleText        AS [Text],
        SequenceOrder
    FROM [dbo].[PropertyHouseRule]
    WHERE PropertyID = @PropertyID
    ORDER BY SequenceOrder;

    -- Result set 3: Safety items (ordered for display)
    SELECT
        ItemText        AS [Text],
        SequenceOrder
    FROM [dbo].[PropertySafetyItem]
    WHERE PropertyID = @PropertyID
    ORDER BY SequenceOrder;

    -- Result set 4: Cancellation policies
    -- Multiple rows per DaysBeforeCheckIn threshold are supported (ordered lines of policy text)
    SELECT
        DaysBeforeCheckIn,
        PolicyText,
        SequenceOrder
    FROM [dbo].[CancellationPolicy]
    WHERE PropertyID = @PropertyID
    ORDER BY DaysBeforeCheckIn, SequenceOrder;
END

