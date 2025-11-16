using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Domain.DataAdapter
{
	public interface IDapperCheckoutDataAdapter : IDapperSqlQueryBase
	{
		CheckoutAggregate? GetCheckoutPropertyDetail(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available);
    }
}