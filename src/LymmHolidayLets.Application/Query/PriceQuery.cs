using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PriceQuery(IDapperPriceDataAdapter priceDataAdapter) : IPriceQuery
    {
        public PriceAggregate GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout)
        {
            return priceDataAdapter.GetPriceDetail(propertyId, checkIn, checkout);
        }
    }
}
