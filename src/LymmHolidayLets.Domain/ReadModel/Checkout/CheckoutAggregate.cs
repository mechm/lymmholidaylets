namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
    /// <summary>
    /// Class <c>CheckoutAggregate/c> Information required to perform a checkout. Includes dates available, property information and previous checkout for property.
    /// </summary>
    public sealed class CheckoutAggregate
	{
		public CheckoutAggregate(Property property, decimal? totalNightlyPrice, IEnumerable<PropertyAdditionalProduct> propertyAdditionalProduct, IEnumerable<PropertyNightCoupon> propertyNightCoupon, Checkout? previousCheckout)
		{
            Property = property;
            TotalNightlyPrice = totalNightlyPrice;
            PropertyAdditionalProduct = propertyAdditionalProduct;
			PropertyNightCoupon = propertyNightCoupon;
			PreviousCheckout = previousCheckout;
		}

		public Property Property { get; }
        public decimal? TotalNightlyPrice { get; }
        public IEnumerable<PropertyAdditionalProduct> PropertyAdditionalProduct { get; }
		public IEnumerable<PropertyNightCoupon> PropertyNightCoupon { get; }	
	
		/// <summary>
		/// Details of any previous checkout made based on property id, checkIn and checkout date
		/// </summary>
		public Checkout? PreviousCheckout { get; }
    }
}
