using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
	public sealed class DapperCheckoutDataAdapter: SqlQueryBase, IDapperCheckoutDataAdapter
	{
		public DapperCheckoutDataAdapter(DbSession session) : base(session)
		{
		}

		public CheckoutAggregate? GetCheckoutPropertyDetail(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available)
        {
            const string procedure = "Checkout_GetByPropertyID_Date";

			try
			{
                CheckoutAggregate checkoutDetail;

                using var sqlConnection = _session.Connection;
                using (var result = sqlConnection.QueryMultiple(procedure, new
                           {
                               propertyId,
                               checkIn,
                               checkOut,
                               available
                           }, _session.Transaction, 
                           commandType: CommandType.StoredProcedure))
                {
                    Property? property = result.ReadSingleOrDefault<Property>();
                    if (property == null) 
                    {
                        return null;
                    }
                    decimal? totalNightlyPrice = result.ReadSingleOrDefault<decimal?>();
                    IEnumerable<PropertyAdditionalProduct> propertyAdditionalProduct = result.Read<PropertyAdditionalProduct>();
                    IEnumerable<PropertyNightCoupon> propertyNightCoupon = result.Read<PropertyNightCoupon>();
                    Checkout? checkout = result.ReadSingleOrDefault<Checkout>();

                    checkoutDetail = new CheckoutAggregate(property, totalNightlyPrice, propertyAdditionalProduct, propertyNightCoupon, checkout);						
                }
                return checkoutDetail;
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException(
					$"An error occurred finding checkout details with the procedure {procedure}", ex);
			}
		}
    }
}
