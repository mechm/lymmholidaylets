-- =============================================
-- Author:		Matt Chambers
-- Create date: 25/06/2023
-- Modified:    2026-02-25
-- Description:	Upserts a checkout record atomically.
--              Uses MERGE to INSERT or UPDATE based on
--              the unique key (PropertyID, CheckIn, CheckOut),
--              preventing duplicate rows under concurrent requests.
-- =============================================
CREATE PROCEDURE [dbo].[Checkout_Upsert]
          @PropertyID tinyint,
          @CheckIn datetime2(0),
          @CheckOut datetime2(0),
          @StripeNightProductID varchar(100),
          @StripeNightDefaultPriceID varchar(100),
          @StripeNightDefaultUnitPrice decimal(7,2),
          @StripeNightCouponID varchar(100) NULL,
          @StripeNightPercentage decimal(5,2) NULL,
          @OverallPrice decimal(7,2)
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [dbo].[Checkout] WITH (HOLDLOCK) AS target
    USING (SELECT @PropertyID AS PropertyID, @CheckIn AS CheckIn, @CheckOut AS CheckOut) AS source
        ON target.[PropertyID] = source.[PropertyID]
        AND target.[CheckIn]   = source.[CheckIn]
        AND target.[CheckOut]  = source.[CheckOut]
    WHEN MATCHED THEN
        UPDATE SET
            [StripeNightProductID]        = @StripeNightProductID,
            [StripeNightDefaultPriceID]   = @StripeNightDefaultPriceID,
            [StripeNightDefaultUnitPrice] = @StripeNightDefaultUnitPrice,
            [StripeNightCouponID]         = @StripeNightCouponID,
            [StripeNightPercentage]       = @StripeNightPercentage,
            [OverallPrice]                = @OverallPrice,
            [Updated]                     = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT ([PropertyID], [CheckIn], [CheckOut],
                [StripeNightProductID], [StripeNightDefaultPriceID], [StripeNightDefaultUnitPrice],
                [StripeNightCouponID], [StripeNightPercentage], [OverallPrice], [Created])
        VALUES (@PropertyID, @CheckIn, @CheckOut,
                @StripeNightProductID, @StripeNightDefaultPriceID, @StripeNightDefaultUnitPrice,
                @StripeNightCouponID, @StripeNightPercentage, @OverallPrice, SYSUTCDATETIME());
END
