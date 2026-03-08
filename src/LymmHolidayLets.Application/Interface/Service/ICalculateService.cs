using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Interface.Service
{
    /// <summary>
    /// Calculates discount percentages based on night coupons.
    /// Abstracted as an interface to allow injection and unit testing.
    /// </summary>
    public interface ICalculateService
    {
        (decimal? percentOff, int nights) CalculateApplicableDiscountPercentage(
            IEnumerable<PropertyNightCoupon> discounts,
            DateOnly checkIn,
            DateOnly checkout);
    }
}
