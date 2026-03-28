using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Domain.Model.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LymmHolidayLets.Application.Service
{
    /// <summary>
    /// Orchestrates the checkout flow: validates input, looks up property availability and pricing,
    /// synchronises with Stripe (product, coupon, session), and persists the checkout record.
    /// All Stripe interactions are delegated to <see cref="IStripeService"/>; this class owns
    /// the sequencing and error handling, not the payment-provider details.
    /// </summary>
    public sealed class CheckoutService(
        ILogger<CheckoutService> logger,
        IOptions<CheckoutOptions> options,
        ICheckoutCommand checkoutCommand,
        ICheckoutQuery checkoutQuery,
        IStripeService stripeService,
        ICalculateService calculateService)
        : ICheckoutService
    {
        /// <summary>
        /// Internal result type for BuildCheckoutDataAsync — holds only application-level types,
        /// no Stripe SDK types.
        /// </summary>
        private sealed record CheckoutData(
            string ProductId,
            string DefaultPriceId,
            string? CouponId,
            decimal? CouponPercentOff,
            IReadOnlyList<PropertyAdditionalProduct> AdditionalProducts,
            string PropertyName,
            decimal NightlyPrice,
            decimal OverallPrice);

        /// <inheritdoc />
        public async Task<CheckoutResponse> CheckoutAsync(
            byte propertyId,
            DateOnly checkIn,
            DateOnly checkout,
            short? numberOfAdults,
            short? numberOfChildren,
            short? numberOfInfants,
            CancellationToken cancellationToken = default)
        {
            // ── Step 1: Validate all input up-front ────────────
            if (!GuestCount.TryCreate(numberOfAdults, numberOfChildren, numberOfInfants, out var guests))
            {
                logger.LogWarning("Checkout rejected — no adults specified for PropertyId={PropertyId}", propertyId);
                return CheckoutResponse.Failure("At least one adult must be specified.");
            }

            if (!DateRange.TryCreate(checkIn, checkout, out var stay))
            {
                logger.LogWarning("Invalid date range for PropertyId={PropertyId}: CheckIn={CheckIn}, CheckOut={CheckOut}",
                    propertyId, checkIn, checkout);
                return CheckoutResponse.Failure("Check-out date must be after the check-in date.");
            }

            try
            {
                // ── Step 2: Build checkout data (property lookup, pricing, Stripe product/coupon) ──
                var (error, data) = await BuildCheckoutDataAsync(propertyId, stay, cancellationToken);
                if (data is null)
                {
                    return FailWithWarning(propertyId, stay, error ?? "Unable to build checkout data.");
                }

                // ── Step 3: Create Stripe checkout session ──────────────────
                var session = await stripeService.CreateSessionAsync(
                    options.Value.BaseUrl, data.PropertyName, data.ProductId, data.DefaultPriceId,
                    data.CouponId, data.AdditionalProducts,
                    propertyId, stay.CheckIn, stay.CheckOut,
                    guests.Adults, guests.Children, guests.Infants,
                    cancellationToken);

                if (session is null)
                {
                    return FailWithWarning(propertyId, stay,
                        "Failed to create payment session. Please try again.");
                }

                // ── Step 4: Persist checkout record ────────────────────────────
                await SafePersistAsync(propertyId, stay, session.Id, data, cancellationToken);

                // ── Step 5: Return success ─────────────────────────────────────
                logger.LogInformation(
                    "Checkout session created — PropertyId={PropertyId}, Stay={Stay}, OverallPrice={OverallPrice}",
                    propertyId, stay, data.OverallPrice);

                return CheckoutResponse.Success(new CheckoutResult
                {
                    SessionId  = session.Id,
                    SessionUrl = session.Url,
                    CheckIn    = stay.CheckIn,
                    CheckOut   = stay.CheckOut
                });
            }
            catch (InvalidCheckoutDataException ex)
            {
                logger.LogWarning(ex,
                    "Invalid checkout data for PropertyId={PropertyId} Stay={Stay}",
                    propertyId, stay);
                return CheckoutResponse.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Unexpected error during checkout for PropertyId={PropertyId} Stay={Stay}",
                    propertyId, stay);
                return CheckoutResponse.Failure("An unexpected error occurred. Please try again.");
            }
        }

        /// <summary>
        /// Looks up property availability and pricing, then syncs with Stripe to get product/coupon IDs.
        /// Does <b>not</b> persist anything — the caller is responsible for that.
        /// </summary>
        /// <returns>
        /// On failure: <c>error</c> contains a user-facing message and <c>data</c> is <c>null</c>.
        /// On success: <c>error</c> is <c>null</c> and <c>data</c> is populated.
        /// </returns>
        private async Task<(string? error, CheckoutData? data)> BuildCheckoutDataAsync(
            byte propertyId, DateRange stay, CancellationToken cancellationToken)
        {
            var lookupResult = checkoutQuery.GetByPropertyIdAndDate(propertyId, stay.CheckIn, stay.CheckOut);

            switch (lookupResult)
            {
                case CheckoutLookupResult.PropertyNotFound:
                    logger.LogWarning("Property not found — PropertyId={PropertyId} does not exist", propertyId);
                    return ($"Property {propertyId} was not found.", null);

                case CheckoutLookupResult.DatesUnavailable unavailable:
                    logger.LogWarning(
                        "No available dates for PropertyId={PropertyId} ({PropertyName}) Stay={Stay}",
                        propertyId, unavailable.PropertyName, stay);
                    return ($"No availability for {stay}. The selected dates may already be booked or blocked.", null);

                case CheckoutLookupResult.Available { Data: var pc }:
                    return await BuildFromAvailableAsync(pc, stay, cancellationToken);

                default:
                    logger.LogError("Unexpected CheckoutLookupResult type: {Type}", lookupResult.GetType().Name);
                    return ("An unexpected error occurred.", null);
            }
        }

        /// <summary>
        /// Handles the happy-path branch of <see cref="BuildCheckoutDataAsync"/> once we know
        /// the property is available. Calculates discount, syncs with Stripe, and totals up prices.
        /// </summary>
        private async Task<(string? error, CheckoutData? data)> BuildFromAvailableAsync(
            CheckoutAggregate propertyCheckout, DateRange stay, CancellationToken cancellationToken)
        {
            // 1. Calculate discount
            var (percentOff, _) = calculateService.CalculateApplicableDiscountPercentage(
                propertyCheckout.PropertyNightCoupon, stay.CheckIn, stay.CheckOut);

            // 2. Prepare Stripe metadata
            var productName        = $"{propertyCheckout.Property.FriendlyName} - {stay}";
            var productDescription = $"Price for {stay.Nights} {(stay.Nights == 1 ? "Night" : "Nights")}";

            // 3. Sync with Stripe — PreviousCheckout enables idempotent reuse of existing Stripe IDs
            var (product, coupon) = await stripeService.CreateProductAndCouponAsync(
                propertyCheckout.PreviousCheckout,
                productName,
                productDescription,
                propertyCheckout.TotalNightlyPrice!.Value,
                percentOff,
                cancellationToken);

            // 4. Calculate final totals (nightly + additional products)
            var additionalProducts = propertyCheckout.PropertyAdditionalProduct.ToList();
            var additionalCharges  = additionalProducts.Sum(p => p.Quantity * p.StripeDefaultUnitPrice);
            var overallPrice       = propertyCheckout.TotalNightlyPrice.Value + additionalCharges;

            return (null, new CheckoutData(
                product.Id,
                product.DefaultPriceId,
                coupon?.Id,
                coupon?.PercentOff,
                additionalProducts,
                propertyCheckout.Property.FriendlyName,
                propertyCheckout.TotalNightlyPrice.Value,
                overallPrice));
        }


        /// <summary>
        /// Persists the checkout record <b>after</b> a Stripe session has been created.
        /// If persistence fails, a <see cref="LogLevel.Critical"/> message is emitted
        /// so the orphaned Stripe session can be reconciled manually — but the checkout
        /// is still considered a success from the user's perspective.
        /// </summary>
        private async Task SafePersistAsync(
            byte propertyId, DateRange stay,
            string sessionId, CheckoutData data, CancellationToken cancellationToken)
        {
            try
            {
                await checkoutCommand.UpsertAsync(new Model.Command.Checkout(
                    propertyId,
                    stay.CheckIn,
                    stay.CheckOut,
                    data.ProductId,
                    data.DefaultPriceId,
                    data.NightlyPrice,
                    data.CouponId,
                    data.CouponPercentOff,
                    data.OverallPrice),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex,
                    "Stripe session {SessionId} was created but checkout record could NOT be persisted " +
                    "for PropertyId={PropertyId} Stay={Stay}. Manual reconciliation required.",
                    sessionId, propertyId, stay);
            }
        }


        /// <summary>
        /// Creates a <see cref="CheckoutResponse.Failure"/> and emits a structured warning log.
        /// Used from <see cref="CheckoutAsync"/> to reduce repetitive logging boilerplate.
        /// </summary>
        private CheckoutResponse FailWithWarning(byte propertyId, DateRange stay, string error)
        {
            logger.LogWarning(
                "Checkout aborted — PropertyId={PropertyId}, Stay={Stay}: {Error}",
                propertyId, stay, error);
            return CheckoutResponse.Failure(error);
        }
    }
}
