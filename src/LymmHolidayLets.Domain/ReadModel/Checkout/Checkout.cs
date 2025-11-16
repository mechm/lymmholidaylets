namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
	public sealed class Checkout
	{
		public required int Id { get; set; }
		public required string StripeNightProductId { get; set; }
		public string? StripeNightCouponId { get; set; }
    }
}
