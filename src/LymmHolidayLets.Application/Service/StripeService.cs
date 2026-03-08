﻿using System.Globalization;
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
	    /// <returns>A tuple of the resolved <see cref="Product"/> and an optional <see cref="Coupon"/>.</returns>
	    public (Product product, Coupon? coupon) CreateProductAndCoupon(Checkout? previousCheckout, string productName, string productDescription, decimal unitAmount, decimal? percentOff) 
		{
            Product nightlyProduct = GetOrCreateProduct(previousCheckout?.StripeNightProductId, productName, productDescription, unitAmount);
            Coupon? nightlyCoupon = GetOrCreateCoupon(previousCheckout?.StripeNightCouponId, productName, percentOff, nightlyProduct.Id);

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
        /// <returns>The created <see cref="Session"/>, or <c>null</c> if creation fails.</returns>
        public Session? CreateSession(
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
            short? numberOfInfants)
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
							{ "NoAdult", numberOfAdults.HasValue ? numberOfAdults.Value.ToString() : "" },
							{ "NoChildren", numberOfChildren.HasValue ? numberOfChildren.Value.ToString() : "" },
						    { "NoInfant", numberOfInfants.HasValue ? numberOfInfants.Value.ToString() : "" }
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

			return sessionService.Create(options);
		}

        /// <summary>
        /// Expires an active Stripe checkout session by ID, preventing the customer from
        /// completing payment on a session that is no longer valid (e.g. dates now unavailable).
        /// </summary>
        /// <param name="sessionId">The Stripe session ID to expire.</param>
        /// <returns>The expired <see cref="Session"/>, or <c>null</c> if expiry failed.</returns>
        public Session? ExpireSession(string sessionId)
        {
            SessionService sessionService = new();

            try
            {
                return sessionService.Expire(sessionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error On Expire Session");
            }
            return null;
        }

        /// <summary>
        /// Retrieves an existing Stripe product by ID and reuses it if the price is unchanged,
        /// or creates a new product if no ID is provided or the unit amount has changed.
        /// </summary>
        /// <param name="id">The existing Stripe product ID, or <c>null</c> to force creation.</param>
        /// <param name="name">The product display name.</param>
        /// <param name="description">The product description.</param>
        /// <param name="unitAmount">The expected nightly unit price in GBP.</param>
        /// <returns>The existing or newly created Stripe <see cref="Product"/>.</returns>
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

        /// <summary>
        /// Fetches a Stripe product by ID.
        /// Throws <see cref="InvalidCheckoutDataException"/> if the product cannot be found,
        /// which will be caught and logged by <see cref="CheckoutService"/>.
        /// </summary>
        /// <param name="id">The Stripe product ID.</param>
        /// <returns>The matching Stripe <see cref="Product"/>.</returns>
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

        /// <summary>
        /// Creates a new Stripe product with an inclusive-tax GBP default price.
        /// The unit amount is converted from pounds to pence (multiplied by 100) as required by the Stripe API.
        /// </summary>
        /// <param name="name">The product display name.</param>
        /// <param name="description">The product description shown on the Stripe checkout page.</param>
        /// <param name="unitAmount">The price in GBP (e.g. 150.00).</param>
        /// <returns>The newly created Stripe <see cref="Product"/>.</returns>
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
        
        /// <summary>
        /// Fetches a Stripe price by ID, used to compare the stored unit amount against
        /// the current nightly rate to determine whether the product needs to be recreated.
        /// Throws <see cref="InvalidCheckoutDataException"/> if the price cannot be found.
        /// </summary>
        /// <param name="id">The Stripe price ID.</param>
        /// <returns>The matching Stripe <see cref="Price"/>.</returns>
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
		// Coupon helpers
		// Stripe coupons are associated with specific products via AppliesTo.
		// Reference: https://stripe.com/docs/api/coupons/create#create_coupon-applies_to-products
		/////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Returns <c>null</c> if no discount applies, reuses an existing coupon if still valid,
		/// or creates a new one if the coupon ID is missing or no longer matches the product/percentage.
		/// </summary>
		/// <param name="couponId">The existing Stripe coupon ID from a previous checkout, if any.</param>
		/// <param name="productName">Used as part of the coupon name when creating a new coupon.</param>
		/// <param name="percentOff">The discount percentage. If <c>null</c>, no coupon is applied.</param>
		/// <param name="productId">The Stripe product ID the coupon must be associated with.</param>
		/// <returns>The resolved <see cref="Coupon"/>, or <c>null</c> if no discount applies.</returns>
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

            // Recreate the coupon if it no longer applies to the current product or the percentage has changed
            if (!coupon.AppliesTo.Products.Contains(productId) || coupon.PercentOff.HasValue && coupon.PercentOff.Value.ToString("n2") != percentOff.Value.ToString("n2"))
			{
				return CreateCoupon(productName + percentOff.Value, percentOff, "forever", new List<string> { productId });
			}
			return coupon;			
		}

		/// <summary>
		/// Fetches a Stripe coupon by ID, expanding the <c>applies_to</c> field so that
		/// the associated products can be validated.
		/// Throws <see cref="InvalidCheckoutDataException"/> if the coupon cannot be found.
		/// </summary>
		/// <param name="id">The Stripe coupon ID.</param>
		/// <returns>The matching Stripe <see cref="Coupon"/>.</returns>
		private static Coupon GetCoupon(string id)
		{
			var service = new CouponService();

			try
			{
				return service.Get(id, new CouponGetOptions
                {
					Expand = ["applies_to"],
                });
			}
			catch (Exception ex)
			{
				throw new InvalidCheckoutDataException($"Error getting coupon by id {id}", ex);
			}
		}

		/// <summary>
		/// Creates a new Stripe coupon with a percentage discount, scoped to a specific product.
		/// The coupon duration is set to <c>forever</c> as Stripe requires a duration even for
		/// one-time payment mode sessions.
		/// </summary>
		/// <param name="name">The display name for the coupon.</param>
		/// <param name="percentage">The percentage to discount off the applicable product price.</param>
		/// <param name="duration">The Stripe duration value (e.g. <c>"forever"</c>, <c>"once"</c>).</param>
		/// <param name="products">The list of Stripe product IDs this coupon applies to.</param>
		/// <returns>The newly created Stripe <see cref="Coupon"/>.</returns>
		private static Coupon CreateCoupon(string name, decimal? percentage, string duration, List<string> products)
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
			return service.Create(options);
		}
	}
}
