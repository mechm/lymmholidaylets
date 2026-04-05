using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperPriceDataAdapter(DbSession session) : SqlQueryBase(session), IDapperPriceDataAdapter
    {
        public PriceAggregate GetPriceDetail(byte propertyId, DateOnly checkIn, DateOnly checkOut)
        {
            const string procedure = "Calendar_Price_GetByPropertyID_Date";

            try
            {
                using var sqlConnection = Session.Connection;
                using var result = sqlConnection.QueryMultiple(procedure, new
                    {
                        propertyId,
                        checkIn,
                        checkOut
                    }, Session.Transaction,
                    commandType: CommandType.StoredProcedure);
                var totalNightlyPrice = result.ReadSingleOrDefault<decimal?>();
                var additionalProduct = result.Read<AdditionalProduct>();
                var nightCoupon = result.Read<PropertyNightCoupon>();
                var priceDetail = new PriceAggregate(totalNightlyPrice, additionalProduct, nightCoupon);

                return priceDetail;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding price details with the procedure {procedure}", ex);
            }
        }
    }
}
