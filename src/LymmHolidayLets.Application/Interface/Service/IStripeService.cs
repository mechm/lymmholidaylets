using LymmHolidayLets.Domain.ReadModel.Checkout;
using Stripe;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Interface.Service
{
	public interface IStripeService	
	{
        Session? CreateSession(string host, string propertyName, Product nightlyProduct, Coupon? nightlyCoupon,
                                      IEnumerable<PropertyAdditionalProduct> additionalProducts,
                                      short propertyId, DateOnly checkIn, DateOnly checkout, short? numberOfAdults, short? numberOfChildren, short? numberOfInfants);

        Session? ExpireSession(string sessionId);
        (Product, Coupon?) CreateProductAndCoupon(Checkout? previousCheckout, string productName, string productDescription, decimal unitAmount, decimal? percentOff);

	}
} 