using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IPriceQuery
    {
        PriceAggregate GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout);
    }
}