using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperPriceDataAdapter : SqlQueryBase, IDapperPriceDataAdapter
    {
        public DapperPriceDataAdapter(DbSession session) : base(session)
        {
        }

        public PriceAggregate GetPriceDetail(byte propertyId, DateOnly checkIn, DateOnly checkOut)
        {
            const string procedure = "Calendar_Price_GetByPropertyID_Date";

            try
            {
                PriceAggregate priceDetail;

                using var sqlConnection = _session.Connection;
                using (var result = sqlConnection.QueryMultiple(procedure, new
                {
                    propertyId,
                    checkIn,
                    checkOut
                }, _session.Transaction,
                commandType: CommandType.StoredProcedure))
                {
                    decimal? totalNightlyPrice = result.ReadSingleOrDefault<decimal?>();
                    IEnumerable<AdditionalProduct> additionalProduct = result.Read<AdditionalProduct>();
                    IEnumerable<PropertyNightCoupon> nightCoupon = result.Read<PropertyNightCoupon>();
                    priceDetail = new PriceAggregate(totalNightlyPrice, additionalProduct, nightCoupon);
                }

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
