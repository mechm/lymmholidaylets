namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
    /// <summary>
    /// Class <c>CheckoutAggregate/c> Information required to perform a checkout. Includes dates available, property information and previous checkout for property.
    /// </summary>
    public sealed class CheckoutAggregate(
	    Property property,
	    decimal? totalNightlyPrice,
	    IEnumerable<PropertyAdditionalProduct> propertyAdditionalProduct,
	    IEnumerable<PropertyNightCoupon> propertyNightCoupon,
	    Checkout? previousCheckout)
    {
	    public Property Property { get; } = property;
		public decimal? TotalNightlyPrice { get; } = totalNightlyPrice;
		public IEnumerable<PropertyAdditionalProduct> PropertyAdditionalProduct { get; } = propertyAdditionalProduct;
		public IEnumerable<PropertyNightCoupon> PropertyNightCoupon { get; } = propertyNightCoupon;

		/// <summary>
		/// Details of any previous checkout made based on property id, checkIn and checkout date
		/// </summary>
		public Checkout? PreviousCheckout { get; } = previousCheckout;
	}
}
