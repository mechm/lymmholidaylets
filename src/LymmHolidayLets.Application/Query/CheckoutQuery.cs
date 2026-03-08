using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Query
{
	public sealed class CheckoutQuery(IDapperCheckoutDataAdapter checkoutDataAdapter) : ICheckoutQuery
	{
		public CheckoutAggregate? GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout, bool available)
		{
			return checkoutDataAdapter.GetCheckoutPropertyDetail(propertyId, checkIn, checkout, available);
		}
    }
}
