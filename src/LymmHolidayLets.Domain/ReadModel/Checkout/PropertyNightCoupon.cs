namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
	public sealed class PropertyNightCoupon
	{
        public short NoOfNight { get; set; }
		public decimal Percentage { get; set; }

        public static decimal? SelectApplicableDiscountPercentage(
            IEnumerable<PropertyNightCoupon> discounts,
            int numberOfNights)
        {
            decimal? percentage = null;
            short? lastNightMatch = null;

            foreach (var discount in discounts)
            {
                if (discount.NoOfNight > numberOfNights ||
                    (lastNightMatch != null && !(lastNightMatch < discount.NoOfNight)))
                {
                    continue;
                }

                lastNightMatch = discount.NoOfNight;
                percentage = discount.Percentage;
            }

            return percentage;
        }
	}
}
