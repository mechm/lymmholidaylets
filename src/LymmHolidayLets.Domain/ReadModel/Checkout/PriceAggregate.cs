namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
    public sealed class PriceAggregate
    {
        public PriceAggregate(decimal? totalNightlyPrice, IEnumerable<AdditionalProduct> additionalProduct, IEnumerable<PropertyNightCoupon> nightCoupon)
        {
            TotalNightlyPrice = totalNightlyPrice;
            AdditionalProduct = additionalProduct;
            NightCoupon = nightCoupon;
        }

        public decimal? TotalNightlyPrice { get; set; }
        public IEnumerable<AdditionalProduct> AdditionalProduct { get; }
        public IEnumerable<PropertyNightCoupon> NightCoupon { get; }
    }

    public sealed class AdditionalProduct
    {
        public required string StripeName { get; set; }
        public decimal StripeDefaultUnitPrice { get; set;}
        public byte Quantity { get; set; }
    }
}
