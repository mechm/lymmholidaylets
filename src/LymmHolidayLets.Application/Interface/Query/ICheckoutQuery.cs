using LymmHolidayLets.Domain.ReadModel.Checkout;
namespace LymmHolidayLets.Application.Interface.Query
{
	public interface ICheckoutQuery
	{
		CheckoutAggregate? GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout, bool available);	
    }
}
