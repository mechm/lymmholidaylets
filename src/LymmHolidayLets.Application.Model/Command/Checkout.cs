namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class Checkout
    {
        public Checkout(byte propertyId, DateOnly checkIn, DateOnly checkOut,
            string stripeNightProductId, string stripeNightDefaultPriceId,
            decimal stripeNightDefaultUnitPrice,
            string? stripeNightCouponId, decimal? stripeNightPercentage,
            decimal overallPrice)
        {
            PropertyId = propertyId;
            CheckIn = checkIn;
            CheckOut = checkOut;
            StripeNightProductId = stripeNightProductId;
            StripeNightDefaultPriceId = stripeNightDefaultPriceId;
            StripeNightDefaultUnitPrice = stripeNightDefaultUnitPrice;
            StripeNightCouponId = stripeNightCouponId;
            StripeNightPercentage = stripeNightPercentage;
            OverallPrice = overallPrice;
        }

        public Checkout(int id, byte propertyId, DateOnly checkIn, DateOnly checkOut,
            string stripeNightProductId, string stripeNightDefaultPriceId,
            decimal stripeNightDefaultUnitPrice,
            string? stripeNightCouponId, decimal? stripeNightPercentage,
            decimal overallPrice)
        {
            Id = id;
            PropertyId = propertyId;
            CheckIn = checkIn;
            CheckOut = checkOut;
            StripeNightProductId = stripeNightProductId;
            StripeNightDefaultPriceId = stripeNightDefaultPriceId;
            StripeNightDefaultUnitPrice = stripeNightDefaultUnitPrice;
            StripeNightCouponId = stripeNightCouponId;
            StripeNightPercentage = stripeNightPercentage;
            OverallPrice = overallPrice;
        }

        public int Id { get; init; }
        public byte PropertyId { get; init; }
        public DateOnly CheckIn { get; init; }
        public DateOnly CheckOut { get; init; }

        public string StripeNightProductId { get; init; }
        public string StripeNightDefaultPriceId { get; init; }
        public decimal StripeNightDefaultUnitPrice { get; init; }
        public string? StripeNightCouponId { get; init; }
        public decimal? StripeNightPercentage { get; init; }
        public decimal OverallPrice { get; init; }

    }
}
