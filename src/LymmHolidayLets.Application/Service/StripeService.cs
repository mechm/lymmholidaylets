using System.Globalization;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Service
{
	/// <summary>
	/// Responsible for all Stripe API interactions: creating and reusing products, coupons,
	/// and checkout sessions, as well as expiring active sessions.
	/// </summary>
	public sealed class StripeService(ILogger<StripeService> logger) : IStripeService
    {
	    /// <summary>
	    /// Retrieves or creates a Stripe <see cref="Product"/> and an optional <see cref="Coupon"/>
	    /// for the nightly rate, based on any previous checkout record for the same property and dates.
	    /// </summary>
	    /// <param name="previousCheckout">
	    /// The previous checkout record for this property/date combination, if one exists.
	    /// Used to reuse existing Stripe product and coupon IDs where possible to avoid duplication.
	    /// </param>
	    /// <param name="productName">Display name for the Stripe product (e.g. "Lymm Cottage - 01/03 to 05/03").</param>
	    /// <param name="productDescription">Short description shown on the Stripe checkout page.</param>
	    /// <param name="unitAmount">The total nightly price in GBP used to create or validate the Stripe price.</param>
	    /// <param name="percentOff">Discount percentage to apply via coupon, or <c>null</c> for no discount.</param>
	    /// <param name="cancellationToken">Token to cancel the operation.</param>
	    /// <returns>A tuple of the resolved <see cref="Product"/> and an optional <see cref="Coupon"/>.</returns>
	    public async Task<(Product product, Coupon? coupon)> CreateProductAndCouponAsync(
            Checkout? previousCheckout,
            string productName,
            string productDescription,
            decimal unitAmount,
            decimal? percentOff,
            CancellationToken cancellationToken = default)
		{
            Product nightlyProduct = await GetOrCreateProductAsync(previousCheckout?.StripeNightProductId, productName, productDescription, unitAmount, cancellationToken);
            Coupon? nightlyCoupon = await GetOrCreateCouponAsync(previousCheckout?.StripeNightCouponId, productName, percentOff, nightlyProduct.Id, cancellationToken);

            return (nightlyProduct, nightlyCoupon);
        }

        // GetOrCreateProduct logic:
        // 1. If no existing product ID, create a new Stripe product
        // 2. If a product ID exists, retrieve it and check whether the price has changed
        // 3. If the price has changed, create a new product with the updated price
        // 4. If the price is unchanged, reuse the existing product

        /// <summary>
        /// Creates a Stripe checkout session for a property booking.
        /// Builds line items from the nightly price product and any additional products,
        /// and applies a discount coupon if one is provided.
        /// </summary>
        /// <param name="host">The base URL of the host, used to construct success and cancel redirect URLs.</param>
        /// <param name="propertyName">The friendly name of the property, stored in session metadata.</param>
        /// <param name="productId">The Stripe product ID for the nightly rate (unused directly — <paramref name="defaultPriceId"/> is used for the line item).</param>
        /// <param name="defaultPriceId">The Stripe price ID attached to the nightly product, used as the session line item.</param>
        /// <param name="couponId">Optional Stripe coupon ID to apply as a discount. Pass <c>null</c> for no discount.</param>
        /// <param name="additionalProducts">Additional products (e.g. cleaning fee, pet fee) to include as extra line items.</param>
        /// <param name="propertyId">The internal property identifier, stored in session metadata.</param>
        /// <param name="checkIn">The check-in date, stored in session metadata.</param>
        /// <param name="checkout">The check-out date, stored in session metadata.</param>
        /// <param name="numberOfAdults">Number of adults, stored in session metadata.</param>
        /// <param name="numberOfChildren">Number of children, stored in session metadata.</param>
        /// <param name="numberOfInfants">Number of infants, stored in session metadata.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The created <see cref="Session"/>, or <c>null</c> if creation fails.</returns>
        public async Task<Session?> CreateSessionAsync(
            string host,
            string propertyName,
            string productId,
            string defaultPriceId,
            string? couponId,
            IEnumerable<PropertyAdditionalProduct> additionalProducts,
            short propertyId,
            DateOnly checkIn,
            DateOnly checkout,
            short? numberOfAdults,
            short? numberOfChildren,
            short? numberOfInfants,
            CancellationToken cancellationToken = default)
        {
            // Build session options — phone collection is required for booking contact purposes
            SessionCreateOptions options = new()
			{
				PhoneNumberCollection = new SessionPhoneNumberCollectionOptions { Enabled = true },

                // Store booking context in Stripe session metadata for use in the webhook handler
				Metadata = new Dictionary<string, string> {
							{ "PropertyID", propertyId.ToString() },
                            { "PropertyName", propertyName },
							{ "CheckInDate", checkIn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
							{ "CheckoutDate", checkout.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
							{ "NoAdult", (numberOfAdults ?? 0).ToString() },
							{ "NoChildren", (numberOfChildren ?? 0).ToString() },
						    { "NoInfant", (numberOfInfants ?? 0).ToString() }
				},
			    Mode = "payment",
				SuccessUrl = host + "/payment/success",
				CancelUrl = host + "/payment/cancel",
				AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },

                // Start with the nightly rate as the primary line item; additional products are appended below
				LineItems =
                [
                    new SessionLineItemOptions
                    {
							Price = defaultPriceId,
							Quantity = 1,
					}
				],

                // Session expires after 30 minutes to prevent stale unpaid sessions accumulating in Stripe
				ExpiresAt  = DateTime.UtcNow + new TimeSpan(0, 30, 0)
            };

            // Append any additional products (e.g. cleaning fee, pet fee) as separate line items
		    foreach (var additional in additionalProducts)
		    {
			    options.LineItems.Add(new SessionLineItemOptions
			    {
				    Price = additional.StripeDefaultPriceID,
				    Quantity = additional.Quantity,
			    });
		    }

            // Apply the discount coupon if one was provided — Stripe does not accept an empty discounts list
            if (couponId != null)
			{
				options.Discounts =
				[
					new SessionDiscountOptions
					{
						Coupon = couponId,
					}
				];
			}

			SessionService sessionService = new();

			return await sessionService.CreateAsync(options, cancellationToken: cancellationToken);
		}

        /// <summary>
        /// Expires an active Stripe checkout session by ID, preventing the customer from
        /// completing payment on a session that is no longer valid (e.g. dates now unavailable).
        /// </summary>
        /// <param name="sessionId">The Stripe session ID to expire.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The expired <see cref="Session"/>, or <c>null</c> if expiry failed.</returns>
        public async Task<Session?> ExpireSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            SessionService sessionService = new();

            try
            {
                return await sessionService.ExpireAsync(sessionId, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error expiring Stripe session {SessionId}", sessionId);
            }
            return null;
        }

        /// <summary>
        /// Retrieves an existing Stripe product by ID and reuses it if the price is unchanged,
        /// or creates a new product if no ID is provided or the unit amount has changed.
        /// </summary>
        private static async Task<Product> GetOrCreateProductAsync(string? id, string name, string description, decimal unitAmount, CancellationToken cancellationToken)
		{
			if (id == null)
			{
				return await CreateProductAsync(name, description, unitAmount, cancellationToken);
			}

			Product product = await GetProductAsync(id, cancellationToken);	
			Price price = await GetPriceAsync(product.DefaultPriceId, cancellationToken);

			return !price.UnitAmount.HasValue || price.UnitAmount.HasValue && price.UnitAmount.Value/100m != unitAmount
				? await CreateProductAsync(name, description, unitAmount, cancellationToken)
				: product;
		}

        /// <summary>
        /// Fetches a Stripe product by ID.
        /// Throws <see cref="InvalidCheckoutDataException"/> if the product cannot be found.
        /// </summary>
        private static async Task<Product> GetProductAsync(string id, CancellationToken cancellationToken)
		{
			var productService = new ProductService();

			try
			{
				return await productService.GetAsync(id, cancellationToken: cancellationToken);
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException($"Error getting stripe product by id {id}", ex);
			}
		}

        /// <summary>
        /// Creates a new Stripe product with an inclusive-tax GBP default price.
        /// The unit amount is converted from pounds to pence (multiplied by 100) as required by the Stripe API.
        /// </summary>
        private static async Task<Product> CreateProductAsync(string name, string description, decimal unitAmount, CancellationToken cancellationToken)
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

			return await productService.CreateAsync(product, cancellationToken: cancellationToken);
		}
        
        /// <summary>
        /// Fetches a Stripe price by ID to compare against the current nightly rate.
        /// Throws <see cref="InvalidCheckoutDataException"/> if the price cannot be found.
        /// </summary>
        private static async Task<Price> GetPriceAsync(string id, CancellationToken cancellationToken)
		{
			var priceService = new PriceService();

			try
			{
				return await priceService.GetAsync(id, cancellationToken: cancellationToken);
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException("Error getting stripe price by id {id}", ex);
			}
		}

		/////////////////////////////////////////////////////////////////////
		// Coupon helpers
		/////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Returns <c>null</c> if no discount applies, reuses an existing coupon if still valid,
		/// or creates a new one if the coupon ID is missing or no longer matches the product/percentage.
		/// </summary>
		private static async Task<Coupon?> GetOrCreateCouponAsync(string? couponId, string productName, decimal? percentOff, string productId, CancellationToken cancellationToken) 
        {
            if (percentOff == null)
			{
				return null;
			}
			if (couponId == null)
			{
				return await CreateCouponAsync(productName + percentOff.Value, percentOff, "forever", new List<string> { productId }, cancellationToken);
			}

            Coupon coupon = await GetCouponAsync(couponId, cancellationToken);

            // Recreate the coupon if it no longer applies to the current product or the percentage has changed
            if (!coupon.AppliesTo.Products.Contains(productId) || coupon.PercentOff.HasValue && coupon.PercentOff.Value.ToString("n2") != percentOff.Value.ToString("n2"))
			{
				return await CreateCouponAsync(productName + percentOff.Value, percentOff, "forever", new List<string> { productId }, cancellationToken);
			}
			return coupon;			
		}

		/// <summary>
		/// Fetches a Stripe coupon by ID, expanding the <c>applies_to</c> field so that
		/// the associated products can be validated.
		/// Throws <see cref="InvalidCheckoutDataException"/> if the coupon cannot be found.
		/// </summary>
		private static async Task<Coupon> GetCouponAsync(string id, CancellationToken cancellationToken)
		{
			var service = new CouponService();

			try
			{
				return await service.GetAsync(id, new CouponGetOptions
                {
					Expand = ["applies_to"],
                },
                cancellationToken: cancellationToken);
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException($"Error getting coupon by id {id}", ex);
			}
		}

		/// <summary>
		/// Creates a new Stripe coupon with a percentage discount, scoped to a specific product.
		/// </summary>
		private static async Task<Coupon> CreateCouponAsync(string name, decimal? percentage, string duration, List<string> products, CancellationToken cancellationToken)
		{
			var options = new CouponCreateOptions
			{
				Name = name,
				PercentOff = percentage,
				Duration = duration,
				AppliesTo = new CouponAppliesToOptions
				{
					Products = products
				},
			};
			var service = new CouponService();
			return await service.CreateAsync(options, cancellationToken: cancellationToken);
		}
	}
}
