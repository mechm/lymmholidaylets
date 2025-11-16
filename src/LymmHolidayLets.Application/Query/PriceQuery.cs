using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PriceQuery: IPriceQuery
    {
        private readonly IDapperPriceDataAdapter _priceDataAdapter;
        public PriceQuery(IDapperPriceDataAdapter priceDataAdapter)
        {
            _priceDataAdapter = priceDataAdapter;
        }

        public PriceAggregate GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout)
        {
            return _priceDataAdapter.GetPriceDetail(propertyId, checkIn, checkout);
        }
    }
}