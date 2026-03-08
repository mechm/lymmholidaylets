using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Domain.Model.Common;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service
{
    public sealed class CheckoutService(
        ILogger<CheckoutService> logger,
        ICheckoutCommand checkoutCommand,
        ICheckoutQuery checkoutQuery,
        IStripeService stripeService,
        ICalculateService calculateService)
        : ICheckoutService
    {
        /// <summary>
        /// Internal result type for BuildCheckoutData — holds only application-level types,
        /// no Stripe SDK types.
        /// </summary>
        private sealed record CheckoutData(
            string ProductId,
            string DefaultPriceId,
            string? CouponId,
            decimal? CouponPercentOff,
            IEnumerable<PropertyAdditionalProduct> AdditionalProducts,
            string PropertyName,
            decimal NightlyPrice,
            decimal OverallPrice);

        public (string? error, CheckoutResult? result) Checkout(
            string host,
            byte propertyId,
            DateOnly checkIn,
            DateOnly checkout,
            short? numberOfAdults,
            short? numberOfChildren,
            short? numberOfInfants)
        {
            try
            {
                var (error, data) = BuildCheckoutData(propertyId, checkIn, checkout);

                if (error is not null || data is null)
                {
                    logger.LogWarning("Checkout aborted for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut}: {Error}",
                        propertyId, checkIn, checkout, error);
                    return (error, null);
                }

                var session = stripeService.CreateSession(
                    host, data.PropertyName, data.ProductId, data.DefaultPriceId,
                    data.CouponId, data.AdditionalProducts,
                    propertyId, checkIn, checkout,
                    numberOfAdults, numberOfChildren, numberOfInfants);

                if (session is null)
                {
                    logger.LogWarning("Stripe session creation returned null for PropertyId={PropertyId}", propertyId);
                    return ("Failed to create payment session. Please try again.", null);
                }

                PersistCheckout(propertyId, checkIn, checkout, data);

                logger.LogInformation("Checkout session created for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut} OverallPrice={OverallPrice}",
                    propertyId, checkIn, checkout, data.OverallPrice);

                return (null, new CheckoutResult
                {
                    SessionId  = session.Id,
                    SessionUrl = session.Url,
                    CheckIn    = checkIn,
                    CheckOut   = checkout
                });
            }
            catch (InvalidCheckoutDataException ex)
            {
                logger.LogError(ex, "Invalid checkout data for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut}",
                    propertyId, checkIn, checkout);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during checkout for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut}",
                    propertyId, checkIn, checkout);
            }

            return ("An unexpected error occurred. Please try again.", null);
        }

        /// <summary>
        /// Fetches property/pricing data and builds Stripe product/coupon IDs.
        /// Does not persist anything — side effect free.
        /// </summary>
        private (string? error, CheckoutData? data) BuildCheckoutData(byte propertyId, DateOnly checkIn, DateOnly checkout)
        {
            var stay = new DateRange(checkIn, checkout);
            var propertyCheckout = checkoutQuery.GetByPropertyIdAndDate(propertyId, stay.CheckIn, stay.CheckOut, available: true);

            if (propertyCheckout is null)
            {
                logger.LogWarning("No property available for PropertyId={PropertyId} Stay={Stay}", propertyId, stay);
                return ("No Property Available", null);
            }

            if (propertyCheckout.TotalNightlyPrice is null)
            {
                logger.LogWarning("No price available for PropertyId={PropertyId} Stay={Stay}", propertyId, stay);
                return ("No Price Available for dates selected, please change date selection", null);
            }

            var productName        = $"{propertyCheckout.Property.FriendlyName} - {stay}";
            var productDescription = $"Price for {stay.Nights} {(stay.Nights == 1 ? "Night" : "Nights")}";

            var (percentOff, _) = calculateService.CalculateApplicableDiscountPercentage(
                propertyCheckout.PropertyNightCoupon, stay.CheckIn, stay.CheckOut);

            var (product, coupon) = stripeService.CreateProductAndCoupon(
                propertyCheckout.PreviousCheckout, productName, productDescription,
                propertyCheckout.TotalNightlyPrice.Value, percentOff);

            var additional   = propertyCheckout.PropertyAdditionalProduct.Sum(p => p.Quantity * p.StripeDefaultUnitPrice);
            var overallPrice = propertyCheckout.TotalNightlyPrice.Value + additional;

            return (null, new CheckoutData(
                product.Id,
                product.DefaultPriceId,
                coupon?.Id,
                coupon?.PercentOff,
                propertyCheckout.PropertyAdditionalProduct,
                propertyCheckout.Property.FriendlyName,
                propertyCheckout.TotalNightlyPrice.Value,
                overallPrice));
        }

        /// <summary>
        /// Persists the checkout record. Separated from BuildCheckoutData
        /// so data building and persistence are distinct responsibilities.
        /// </summary>
        private void PersistCheckout(byte propertyId, DateOnly checkIn, DateOnly checkout, CheckoutData data)
        {
            checkoutCommand.Upsert(new Model.Command.Checkout(
                propertyId,
                checkIn,
                checkout,
                data.ProductId,
                data.DefaultPriceId,
                data.NightlyPrice,
                data.CouponId,
                data.CouponPercentOff,
                data.OverallPrice));

            logger.LogInformation("Checkout upserted for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut}",
                propertyId, checkIn, checkout);
        }
    }
}
