using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Service
{
    public sealed class CalculateService : ICalculateService
    {
        /// <inheritdoc />
        public (decimal? percentOff, int nights) CalculateApplicableDiscountPercentage(
            IEnumerable<PropertyNightCoupon> discounts, DateOnly checkIn, DateOnly checkout)
            => Calculate(discounts, checkIn, checkout);

        /// <summary>
        /// Static entry point kept for use in unit tests and places that do not use DI.
        /// </summary>
        public static (decimal?, int) Calculate(IEnumerable<PropertyNightCoupon> discounts, DateOnly checkIn, DateOnly checkout)
        {
            var noOfNights = checkout.DayNumber - checkIn.DayNumber;

            decimal? percentage = null;
            short? lastNightMatch = null;

            foreach (var discount in discounts)
            {
                if (discount.NoOfNight > noOfNights ||
                    (lastNightMatch != null && !(lastNightMatch < discount.NoOfNight))) continue;
                lastNightMatch = discount.NoOfNight;
                percentage = discount.Percentage;
            }

            return (percentage, noOfNights);
        }
    }
}


