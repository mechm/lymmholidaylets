using LymmHolidayLets.Domain.ReadModel.Checkout;
using Stripe;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Interface.Service
{
    public interface IStripeService
    {
        /// <summary>
        /// Creates a Stripe checkout session using pre-built product/price/coupon IDs.
        /// Accepts plain string IDs rather than Stripe SDK objects to keep callers
        /// decoupled from the Stripe SDK.
        /// </summary>
        Session? CreateSession(
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
            short? numberOfInfants);

        Session? ExpireSession(string sessionId);

        (Product product, Coupon? coupon) CreateProductAndCoupon(
            Checkout? previousCheckout,
            string productName,
            string productDescription,
            decimal unitAmount,
            decimal? percentOff);
    }
}

