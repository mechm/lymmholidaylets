-- =============================================
-- Author:		Matt Chambers
-- Create date: 25/06/2023
-- Description:	Updates a previous checkout
-- =============================================
CREATE PROCEDURE [dbo].[Checkout_Update]	   
          @ID int,
          @PropertyID tinyint,
          @CheckIn datetime2(0),
          @CheckOut datetime2(0),
          @StripeNightProductID varchar(100),
          @StripeNightDefaultPriceID varchar(100),
          @StripeNightDefaultUnitPrice decimal(7,2),
          @StripeNightCouponID varchar(100) NULL,
          @StripeNightPercentage decimal(5,2) NULL,
          @OverallPrice decimal(7,2),
          @Updated datetime2(0)
AS
BEGIN
   SET NOCOUNT ON;

    UPDATE [dbo].[Checkout]
    SET [PropertyID] = @PropertyID
      ,[CheckIn] = @CheckIn
      ,[CheckOut] = @CheckOut
      ,[StripeNightProductID] = @StripeNightProductID
      ,[StripeNightDefaultPriceID] = @StripeNightDefaultPriceID
      ,[StripeNightDefaultUnitPrice] = @StripeNightDefaultUnitPrice
      ,[StripeNightCouponID] = @StripeNightCouponID
      ,[StripeNightPercentage] = @StripeNightPercentage
      ,[OverallPrice] = @OverallPrice
      ,[Updated] = @Updated
    WHERE [ID] = @ID

END