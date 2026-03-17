using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Domain.Model.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Service
{
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
                    return FailWithWarning(propertyId, checkIn, checkout, error ?? "Unable to build checkout data.");
                }

                // ── Step 3: Create Stripe session ──────────────────────────────
                var session = await CreateStripeSessionAsync(propertyId, stay, guests, data, cancellationToken);
                if (session is null)
                {
                    return FailWithWarning(propertyId, checkIn, checkout,
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

        // ──────────────────────────────────────────────────────────────────────
        //  Private helpers — each handles a single responsibility
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Fetches property/pricing data and creates the corresponding Stripe product &amp; coupon.
        /// Returns <c>(error, null)</c> on failure or <c>(null, data)</c> on success.
        /// Accepts a pre-validated <see cref="DateRange"/> — date validation is performed
        /// in <see cref="CheckoutAsync"/> before this method is called.
        /// Pure aside from the Stripe API call — does <b>not</b> persist anything.
        /// </summary>
        private async Task<(string? error, CheckoutData? data)> BuildCheckoutDataAsync(
            byte propertyId, DateRange stay, CancellationToken cancellationToken)
        {

            var lookupResult = checkoutQuery.GetByPropertyIdAndDate(propertyId, stay.CheckIn, stay.CheckOut);

            return lookupResult switch
            {
                CheckoutLookupResult.PropertyNotFound =>
                    LogAndFail($"Property {propertyId} was not found.",
                        "Property not found — PropertyId={PropertyId} does not exist", propertyId),

                CheckoutLookupResult.DatesUnavailable unavailable =>
                    LogAndFail($"No availability for {stay}. The selected dates may already be booked or blocked.",
                        "No available dates for PropertyId={PropertyId} ({PropertyName}) Stay={Stay}",
                        propertyId, unavailable.PropertyName, stay),

                CheckoutLookupResult.Available { Data: var pc } =>
                    await BuildFromAvailableAsync(pc, stay, cancellationToken),

                _ => LogAndFailError("Unexpected CheckoutLookupResult type: {Type}", lookupResult.GetType().Name)
            };
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
        /// Creates a Stripe checkout session from the pre-built checkout data and guest counts.
        /// BaseUrl comes from <see cref="CheckoutOptions"/> so the application layer has
        /// no dependency on the HTTP request context.
        /// </summary>
        private Task<Session?> CreateStripeSessionAsync(
            byte propertyId, DateRange stay,
            GuestCount guests, CheckoutData data, CancellationToken cancellationToken)
        {
            var host = options.Value.BaseUrl;

            return stripeService.CreateSessionAsync(
                host, data.PropertyName, data.ProductId, data.DefaultPriceId,
                data.CouponId, data.AdditionalProducts,
                propertyId, stay.CheckIn, stay.CheckOut,
                guests.Adults, guests.Children, guests.Infants,
                cancellationToken);
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

                logger.LogInformation(
                    "Checkout upserted for PropertyId={PropertyId} Stay={Stay}",
                    propertyId, stay);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex,
                    "Stripe session {SessionId} was created but checkout record could NOT be persisted " +
                    "for PropertyId={PropertyId} Stay={Stay}. Manual reconciliation required.",
                    sessionId, propertyId, stay);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  Logging helpers — reduce repetition in BuildCheckoutDataAsync
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Logs a warning and returns a failure tuple for <see cref="BuildCheckoutDataAsync"/>.</summary>
        private (string? error, CheckoutData? data) LogAndFail(
            string userMessage, string logTemplate, params object[] logArgs)
        {
            logger.LogWarning(logTemplate, logArgs);
            return (userMessage, null);
        }

        /// <summary>Logs an error and returns a failure tuple for unexpected cases.</summary>
        private (string? error, CheckoutData? data) LogAndFailError(
            string logTemplate, params object[] logArgs)
        {
            logger.LogError(logTemplate, logArgs);
            return ("An unexpected error occurred.", null);
        }

        /// <summary>
        /// Creates a <see cref="CheckoutResponse.Failure"/> and emits a structured warning log.
        /// Used from <see cref="CheckoutAsync"/> to reduce repetitive logging boilerplate.
        /// </summary>
        private CheckoutResponse FailWithWarning(byte propertyId, DateOnly checkIn, DateOnly checkout, string error)
        {
            logger.LogWarning(
                "Checkout aborted — PropertyId={PropertyId}, CheckIn={CheckIn}, CheckOut={CheckOut}: {Error}",
                propertyId, checkIn, checkout, error);
            return CheckoutResponse.Failure(error);
        }
    }
}
