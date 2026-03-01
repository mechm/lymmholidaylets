using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Domain.Model.Common;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Service
{
    public sealed class CheckoutService(
        ILogger<CheckoutService> logger,
        IManageCheckoutSessionService manageCheckoutSessionService,
        ICheckoutCommand checkoutCommand,
        ICheckoutQuery checkoutQuery,
        IStripeService stripeService)
        : ICheckoutService
    {
        /// <summary>
        /// Internal result type for BuildCheckoutData to avoid long tuple returns.
        /// </summary>
        private sealed record CheckoutData(
            Product Product,
            Coupon? Coupon,
            IEnumerable<PropertyAdditionalProduct> AdditionalProducts,
            string PropertyName,
            decimal NightlyPrice,
            decimal OverallPrice);

        public (string? error, Session? session) Checkout(string host, byte propertyId, DateOnly checkIn, DateOnly checkout, short? numberOfAdults, short? numberOfChildren, short? numberOfInfants, bool available = true)
        {
            try
            {
                var (error, data) = BuildCheckoutData(propertyId, checkIn, checkout, available);

                if (error is not null || data is null)
                {
                    logger.LogWarning("Checkout aborted for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut}: {Error}",
                        propertyId, checkIn, checkout, error);
                    return (error, null);
                }

                Session? session = stripeService.CreateSession(host, data.PropertyName, data.Product, data.Coupon,
                    data.AdditionalProducts, propertyId, checkIn, checkout, numberOfAdults, numberOfChildren, numberOfInfants);

                if (session is null)
                {
                    logger.LogWarning("Stripe session creation returned null for PropertyId={PropertyId}", propertyId);
                    return ("Failed to create payment session. Please try again.", null);
                }

                PersistCheckout(propertyId, checkIn, checkout, data);
                manageCheckoutSessionService.AddUpdateSessionCache(session, checkIn, checkout);

                logger.LogInformation("Checkout session created successfully for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut} OverallPrice={OverallPrice}",
                    propertyId, checkIn, checkout, data.OverallPrice);

                return (null, session);
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
        /// Fetches property/pricing data and creates the Stripe product and coupon.
        /// Does not persist anything — side effect free.
        /// </summary>
        private (string? error, CheckoutData? data) BuildCheckoutData(byte propertyId, DateOnly checkIn, DateOnly checkout, bool available)
        {
            var stay = new DateRange(checkIn, checkout);
            CheckoutAggregate? propertyCheckout = checkoutQuery.GetByPropertyIdAndDate(propertyId, stay.CheckIn, stay.CheckOut, available);

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

            var productName = $"{propertyCheckout.Property.FriendlyName} - {stay}";
            var productDescription = $"Price for {stay.Nights} {(stay.Nights == 1 ? "Night" : "Nights")}";

            (decimal? percentOff, _) = CalculateService.CalculateApplicableDiscountPercentage(
                propertyCheckout.PropertyNightCoupon, stay.CheckIn, stay.CheckOut);

            (Product product, Coupon? coupon) = stripeService.CreateProductAndCoupon(
                propertyCheckout.PreviousCheckout, productName, productDescription,
                propertyCheckout.TotalNightlyPrice.Value, percentOff);

            decimal additional = propertyCheckout.PropertyAdditionalProduct.Sum(p => p.Quantity * p.StripeDefaultUnitPrice);
            decimal overallPrice = propertyCheckout.TotalNightlyPrice.Value + additional;

            return (null, new CheckoutData(product, coupon, propertyCheckout.PropertyAdditionalProduct,
                propertyCheckout.Property.FriendlyName, propertyCheckout.TotalNightlyPrice.Value, overallPrice));
        }

        /// <summary>
        /// Persists the checkout record to the database. Separated from BuildCheckoutData
        /// so data building and persistence are distinct responsibilities.
        /// </summary>
        private void PersistCheckout(byte propertyId, DateOnly checkIn, DateOnly checkout, CheckoutData data)
        {
            checkoutCommand.Upsert(new Model.Command.Checkout(
                propertyId,
                checkIn,
                checkout,
                data.Product.Id,
                data.Product.DefaultPriceId,
                data.NightlyPrice,
                data.Coupon?.Id,
                data.Coupon?.PercentOff,
                data.OverallPrice));

            logger.LogInformation("Checkout upserted for PropertyId={PropertyId} CheckIn={CheckIn} CheckOut={CheckOut}",
                propertyId, checkIn, checkout);
        }
    }
}
