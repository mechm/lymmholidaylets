using LymmHolidayLets.Application.Model.Service;

namespace LymmHolidayLets.Application.Interface.Service
{
    public interface ICheckoutService
    {
        (string? error, CheckoutResult? result) Checkout(
            string host,
            byte propertyId,
            DateOnly checkIn,
            DateOnly checkout,
            short? numberOfAdults,
            short? numberOfChildren,
            short? numberOfInfants);
    }
}


