namespace LymmHolidayLets.Application.Interface.Service
{
	public interface ICheckoutService
	{
		(string?, Stripe.Checkout.Session?) Checkout(string host, byte propertyId, DateOnly checkIn, DateOnly checkout, short? numberOfAdult, short? noChildren, short? noInfant, bool available = true);
	}
}
