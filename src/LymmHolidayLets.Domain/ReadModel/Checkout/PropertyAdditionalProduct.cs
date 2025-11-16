namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
    public sealed class PropertyAdditionalProduct
    {
		public required string StripeDefaultPriceID { get; set; }
		public decimal StripeDefaultUnitPrice { get; set; }
		public byte Quantity { get; set; }
	}
}
