-- =============================================
-- Author:		Matt Chambers
-- Create date: 24/09/2023
-- Description:	Obtains Prices based on Dates and Property ID
-- =============================================
CREATE PROCEDURE [dbo].[Calendar_Price_GetByPropertyID_Date]
	@PropertyID tinyint,
	@CheckIn date,
	@CheckOut date
AS
BEGIN
	  SET NOCOUNT ON;

	  SELECT SUM(coalesce(c.[Price], p.DefaultNightlyPrice)) as TotalNightlyPrice
	  FROM [dbo].[Calendar] c WITH (nolock)
	  INNER JOIN [dbo].[Property] p WITH (nolock) on p.ID = c.PropertyID
	  WHERE PropertyID = @PropertyID
	  AND c.[Date] >= @CheckIn AND c.[Date] < @CheckOut
	  
	  SELECT [StripeName],[StripeDefaultUnitPrice],[Quantity]
	  FROM [dbo].[PropertyAdditionalProduct]WITH (nolock)
	  WHERE PropertyID = @PropertyID

	  SELECT [NoOfNight],[Percentage]
	  FROM [dbo].[PropertyNightCoupon] WITH (nolock)
	  WHERE PropertyID = @PropertyID

END