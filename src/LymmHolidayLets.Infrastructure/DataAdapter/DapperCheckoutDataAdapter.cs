using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
	public sealed class DapperCheckoutDataAdapter(DbSession session) : SqlQueryBase(session), IDapperCheckoutDataAdapter
	{
		public CheckoutAggregate? GetCheckoutPropertyDetail(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available)
        {
            const string procedure = "Checkout_GetByPropertyID_Date";

			try
			{
				using var sqlConnection = Session.Connection;
				using var result = sqlConnection.QueryMultiple(procedure, new
					{
						propertyId,
						checkIn,
						checkOut,
						available
					}, Session.Transaction, 
					commandType: CommandType.StoredProcedure);
				Property? property = result.ReadSingleOrDefault<Property>();
                if (property == null) 
                {
	                return null;
                }

                decimal? totalNightlyPrice = result.ReadSingleOrDefault<decimal?>();

                // When no nightly price exists the dates are unavailable — the remaining
                // result sets (additional products, coupons, previous checkout) are only
                // needed for the happy path, so return early with minimal data.
                if (totalNightlyPrice is null)
                {
                    return new CheckoutAggregate(property, null,
                        [],
                        [],
                        null);
                }

                IEnumerable<PropertyAdditionalProduct> propertyAdditionalProduct = result.Read<PropertyAdditionalProduct>();
                IEnumerable<PropertyNightCoupon> propertyNightCoupon = result.Read<PropertyNightCoupon>();
                Checkout? checkout = result.ReadSingleOrDefault<Checkout>();

                return new CheckoutAggregate(property, totalNightlyPrice, propertyAdditionalProduct, propertyNightCoupon, checkout);
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException(
					$"An error occurred finding checkout details with the procedure {procedure}", ex);
			}
		}
    }
}
