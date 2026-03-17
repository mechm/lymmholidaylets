using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;

namespace LymmHolidayLets.Application.Query
{
	public sealed class CheckoutQuery(IDapperCheckoutDataAdapter checkoutDataAdapter) : ICheckoutQuery
	{
		/// <summary>
		/// Looks up property, pricing, additional products, night coupons, and previous checkout
		/// for the given property and date range. Availability is always checked (Available = true).
		/// </summary>
		public CheckoutLookupResult GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout)
		{
			var aggregate = checkoutDataAdapter.GetCheckoutPropertyDetail(propertyId, checkIn, checkout, available: true);

			if (aggregate is null)
				return new CheckoutLookupResult.PropertyNotFound();

			if (aggregate.TotalNightlyPrice is null)
				return new CheckoutLookupResult.DatesUnavailable(aggregate.Property.FriendlyName);

			return new CheckoutLookupResult.Available(aggregate);
		}
    }
}
