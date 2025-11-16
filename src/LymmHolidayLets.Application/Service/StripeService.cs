using System.Globalization;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Stripe;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Service
{
	/// <summary>
	/// Class <c>StripeService</c> responsible for creating a checkout session and interaction with stripe API
	/// </summary>
	public sealed class StripeService(ILogger logger) : IStripeService
    {
        private readonly ILogger _logger = logger;

        public (Product, Coupon?) CreateProductAndCoupon(Checkout? previousCheckout, string productName, string productDescription, decimal unitAmount, decimal? percentOff) 
		{
            Product nightlyProduct = GetOrCreateProduct(previousCheckout?.StripeNightProductId, productName, productDescription, unitAmount);
            Coupon? nightlyCoupon = GetOrCreateCoupon(previousCheckout?.StripeNightCouponId, productName, percentOff, nightlyProduct.Id);

            return (nightlyProduct, nightlyCoupon);
        }

		// 1 check if product exists
		// 2 check if product price changed
		// 3 if changed update else continue with product
		// if not exists create product and save to db and continue

        /// <summary>
        /// Class <c>StripeService</c> responsible for creating a checkout session and interaction with stripe API
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="nightlyCoupon"></param>
        /// <param name="additionalProducts"></param>
        /// <param name="propertyName"></param>
        /// <param name="nightlyProduct"></param>
        /// <param name="propertyId"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkout"></param>
        /// <param name="numberOfAdults"></param>
        /// <param name="numberOfChildren"></param>
        /// <param name="numberOfInfants"></param>
        /// <returns>
        /// A string representing a point's location, in the form (x,y),
        /// without any leading, trailing, or embedded whitespace.
        /// </returns>
        public Session? CreateSession(string host, string propertyName, Product nightlyProduct, Coupon? nightlyCoupon,
                                      IEnumerable<PropertyAdditionalProduct> additionalProducts, 
									  short propertyId, DateOnly checkIn, DateOnly checkout, short? numberOfAdults, short? numberOfChildren, short? numberOfInfants)
		{
            // Setup the session options for the session for stripe
            SessionCreateOptions options = new()
			{
				PhoneNumberCollection = new SessionPhoneNumberCollectionOptions { Enabled = true },
				Metadata = new Dictionary<string, string> {
							{ "PropertyID", propertyId.ToString() },
                            { "PropertyName", propertyName },
							{ "CheckInDate", checkIn.ToString(CultureInfo.CurrentCulture) },
							{ "CheckoutDate", checkout.ToString(CultureInfo.CurrentCulture) },
							{ "NoAdult", numberOfAdults.HasValue ? numberOfAdults.Value.ToString() : "" },
							{ "NoChildren", numberOfChildren.HasValue ? numberOfChildren.Value.ToString() : "" },
						    { "NoInfant", numberOfInfants.HasValue ? numberOfInfants.Value.ToString() : "" }
				},
			    Mode = "payment",
				SuccessUrl = host + "/payment/success",
				CancelUrl = host + "/payment/cancel",
				AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
				LineItems =
                [
                    new()
                    {
							Price = nightlyProduct.DefaultPriceId,
							Quantity = 1,
					}
				],
				ExpiresAt  = DateTime.UtcNow + new TimeSpan(0, 30, 0)
            };
			
		    foreach (var additional in additionalProducts)
		    {
			    options.LineItems.Add(new SessionLineItemOptions
			    {
				    Price = additional.StripeDefaultPriceID,
				    Quantity = additional.Quantity,
			    });
		    }
            
            if (nightlyCoupon != null)
			{
				options.Discounts = new List<SessionDiscountOptions>
				{
					new()
                    {
						Coupon = nightlyCoupon.Id,
					},
				};
			}

			SessionService sessionService = new();

			return sessionService.Create(options);
		}

        public Session? ExpireSession(string sessionId)
        {
            SessionService sessionService = new();

            try
            {
                return sessionService.Expire(sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error On Expire Session", ex);
            }
            return null;
        }


        /// <summary>
        /// This method changes the point's location to
        /// the given coordinates.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="unitAmount"></param>
        /// <returns>
        /// A string representing a point's location, in the form (x,y),
        /// without any leading, trailing, or embedded whitespace.
        /// </returns>
        private static Product GetOrCreateProduct(string? id, string name, string description, decimal unitAmount)
		{
			if (id == null)
			{
				return CreateProduct(name, description, unitAmount);
			}

			Product product = GetProduct(id);	
			Price price = GetPrice(product.DefaultPriceId);

			return !price.UnitAmount.HasValue || price.UnitAmount.HasValue && price.UnitAmount.Value/100m != unitAmount
				? CreateProduct(name, description, unitAmount)
				: product;
		}

		private static Product GetProduct(string id)
		{
			var productService = new ProductService();

			try
			{
				return productService.Get(id);
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException($"Error getting stripe product by id {id}", ex);
			}
		}

		private static Product CreateProduct(string name, string description, decimal unitAmount)
		{
			var productService = new ProductService();			

			var product = new ProductCreateOptions
			{
				Name = name,
				Description = description,
				DefaultPriceData = new ProductDefaultPriceDataOptions
				{
					TaxBehavior = "inclusive",
					UnitAmountDecimal = unitAmount * 100,
					Currency = "gbp",
				}
			};

			return productService.Create(product);
		}
		private static Price GetPrice(string id)
		{
			var priceService = new PriceService();

			try
			{
				return priceService.Get(id);
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException("Error getting stripe price by id {id}", ex);
			}
		}

		/////////////////////////////////////////////////////////////////////
		// Create discount coupon and associate with product
		// https://stripe.com/docs/api/coupons/create#create_coupon-applies_to-products					
		/////////////////////////////////////////////////////////////////////

		private static Coupon? GetOrCreateCoupon(string? couponId, string productName, decimal? percentOff, string productId) 
        {
            if (percentOff == null)
			{
				return null;
			}
			if (couponId == null)
			{
				return CreateCoupon(productName + percentOff.Value, percentOff, "forever", new List<string> { productId });
			}

            Coupon coupon = GetCoupon(couponId);

            if (!coupon.AppliesTo.Products.Contains(productId) || coupon.PercentOff.HasValue && coupon.PercentOff.Value.ToString("n2") != percentOff.Value.ToString("n2"))
			{
				return CreateCoupon(productName + percentOff.Value, percentOff, "forever", new List<string> { productId });
			}
			return coupon;			
		}

		private static Coupon GetCoupon(string id)
		{
			var service = new CouponService();

			try
			{
				return service.Get(id, new CouponGetOptions
                {
					Expand = new List<string>{ "applies_to" },
                });
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException($"Error getting coupon by id {id}", ex);
			}
		}

		private static Coupon CreateCoupon(string name, decimal? percentage, string duration, List<string> products)
		{
			var options = new CouponCreateOptions
			{
				//Name = name,
				PercentOff = percentage,
				Duration = duration,
				AppliesTo = new CouponAppliesToOptions
				{
					Products = products
				},
			};
			var service = new CouponService();
			return service.Create(options);
		}
	}
}
