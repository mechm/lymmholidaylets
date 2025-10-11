-- =============================================
-- Author:		Matt Chambers
-- Create date: 25/06/2023
-- Description:	Saves a new checkout
-- =============================================
CREATE PROCEDURE Checkout_Insert	      
          @PropertyID tinyint,
          @CheckIn datetime2(0),
          @CheckOut datetime2(0),
          @StripeNightProductID varchar(100),
          @StripeNightDefaultPriceID varchar(100),
          @StripeNightDefaultUnitPrice decimal(7,2),
          @StripeNightCouponID varchar(100) NULL,
          @StripeNightPercentage decimal(5,2) NULL,
          @OverallPrice decimal(7,2),
          @Created datetime2(0)
AS
BEGIN
   SET NOCOUNT ON;

   INSERT INTO [dbo].[Checkout]
            ([PropertyID]
           ,[CheckIn]
           ,[CheckOut]
           ,[StripeNightProductID]
           ,[StripeNightDefaultPriceID]
           ,[StripeNightDefaultUnitPrice]
           ,[StripeNightCouponID]
           ,[StripeNightPercentage]
           ,[OverallPrice]
           ,[Created])
   VALUES
           (@PropertyID,
           @CheckIn,
           @CheckOut,
           @StripeNightProductID,
           @StripeNightDefaultPriceID,
           @StripeNightDefaultUnitPrice,
           @StripeNightCouponID,
           @StripeNightPercentage,
           @OverallPrice,
           @Created)

END
GO
