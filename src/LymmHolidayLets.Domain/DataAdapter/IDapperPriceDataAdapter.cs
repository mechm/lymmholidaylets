using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperPriceDataAdapter : IDapperSqlQueryBase
    {
        PriceAggregate GetPriceDetail(byte propertyId, DateOnly checkIn, DateOnly checkOut);
    }
}
