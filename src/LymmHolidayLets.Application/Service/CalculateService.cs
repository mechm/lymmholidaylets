namespace LymmHolidayLets.Application.Service
{
	public static class CalculateService
	{
		public static (decimal?, int) CalculateApplicableDiscountPercentage(IEnumerable<Domain.ReadModel.Checkout.PropertyNightCoupon> discounts, DateOnly checkIn, DateOnly checkout) 
		{
			int noOfNights = checkout.DayNumber - checkIn.DayNumber;

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
