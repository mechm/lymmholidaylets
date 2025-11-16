using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Query
{
	public sealed class CheckoutQuery : ICheckoutQuery
	{
		private readonly IDapperCheckoutDataAdapter _checkoutDataAdapter;
		public CheckoutQuery(IDapperCheckoutDataAdapter checkoutDataAdapter) 
		{
			_checkoutDataAdapter = checkoutDataAdapter;
		}

		public CheckoutAggregate? GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout, bool available)
		{
			return _checkoutDataAdapter.GetCheckoutPropertyDetail(propertyId, checkIn, checkout, available);
		}
    }
}
