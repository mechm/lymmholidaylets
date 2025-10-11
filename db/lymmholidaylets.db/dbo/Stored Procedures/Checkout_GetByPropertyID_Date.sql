-- =============================================
-- Author:		Matt Chambers
-- Create date: 25/06/2023
-- Description:	Obtains Calendar, Stripe details from previous Checkout
-- =============================================
CREATE PROCEDURE [dbo].[Checkout_GetByPropertyID_Date]
	@PropertyID tinyint,
	@CheckIn date,
	@CheckOut date,
	@Available bit
AS
BEGIN
	  SET NOCOUNT ON;

	  SELECT TOP 1 FriendlyName FROM [dbo].[Property] p WITH (nolock)
	  WHERE p.ID = @PropertyID

	  SELECT SUM(coalesce(c.[Price], p.DefaultNightlyPrice)) as TotalNightlyPrice
	  FROM [dbo].[Calendar] c WITH (nolock)
	  INNER JOIN [dbo].[Property] p WITH (nolock) on p.ID = c.PropertyID
	  WHERE PropertyID = @PropertyID
	  AND c.[Date] >= @CheckIn AND c.[Date] < @CheckOut AND [Available] = @Available

	  SELECT [StripeProductID],[StripeName]
		  ,[StripeDescription],[StripeDefaultPriceID],
		  [StripeDefaultUnitPrice],[Quantity]
	  FROM [dbo].[PropertyAdditionalProduct] WITH (nolock)
	  WHERE PropertyID = @PropertyID

	  SELECT [NoOfNight],[Percentage]
	  FROM [dbo].[PropertyNightCoupon] WITH (nolock)
	  WHERE PropertyID = @PropertyID

	  SELECT [ID],[StripeNightProductID],[StripeNightCouponID]
	  FROM [dbo].[Checkout] WITH (nolock)
	  WHERE PropertyID = @PropertyID
	  AND [CheckIn] = @CheckIn AND [CheckOut] = @CheckOut

END