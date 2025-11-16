using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.Checkout.Entity
{
	public sealed class Checkout : IAggregateRoot
	{

		// create
		public Checkout(byte propertyID, DateOnly checkIn, DateOnly checkOut,
			string stripeNightProductID, string stripeNightDefaultPriceID,
			decimal stripeNightDefaultUnitPrice,
			string? stripeNightCouponID, decimal? stripeNightPercentage,
			decimal overallPrice) 
		{
			PropertyID = propertyID;
			CheckIn = checkIn;
			CheckOut = checkOut;
			StripeNightProductID = stripeNightProductID;
			StripeNightDefaultPriceID = stripeNightDefaultPriceID;
			StripeNightDefaultUnitPrice = stripeNightDefaultUnitPrice;
			StripeNightCouponID = stripeNightCouponID;
			StripeNightPercentage = stripeNightPercentage;
			OverallPrice = overallPrice;
			Created = DateTime.UtcNow;
		}

		// update
		public Checkout(int id, byte propertyID, DateOnly checkIn, DateOnly checkOut,
			string stripeNightProductID, string stripeNightDefaultPriceID,
			decimal stripeNightDefaultUnitPrice,
			string? stripeNightCouponID, decimal? stripeNightPercentage,
			decimal overallPrice)
		{
			ID = id;
			PropertyID = propertyID;
			CheckIn = checkIn;
			CheckOut = checkOut;
			StripeNightProductID = stripeNightProductID;
			StripeNightDefaultPriceID = stripeNightDefaultPriceID;
			StripeNightDefaultUnitPrice = stripeNightDefaultUnitPrice;
			StripeNightCouponID = stripeNightCouponID;
			StripeNightPercentage = stripeNightPercentage;
			OverallPrice = overallPrice;
			Updated = DateTime.UtcNow;
		}


		public int ID { get; init; }
		public byte PropertyID { get; init; }
        public DateOnly CheckIn { get; init; }
        public DateOnly CheckOut { get; init; }

		public string StripeNightProductID { get; init; }
		public string StripeNightDefaultPriceID { get; init; }
		public decimal StripeNightDefaultUnitPrice { get; init; }
		public string? StripeNightCouponID { get; init; }
		public decimal? StripeNightPercentage { get; init; }
		public decimal OverallPrice { get; init; }
		public DateTime Created { get; init; }
		public DateTime? Updated { get; init; }
	}
}
