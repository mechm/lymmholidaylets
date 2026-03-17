using Dapper;
using LymmHolidayLets.Domain.Model.Checkout.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperCheckoutRepository(DbSession session)
        : RepositoryBase<Checkout>(session), ICheckoutRepository
    {
        public async Task UpsertAsync(Checkout checkout, CancellationToken cancellationToken = default)
        {
            const string procedure = "Checkout_Upsert";

            try
            {
                using var connection = Session.Connection;
                var command = new CommandDefinition(
                    procedure,
                    new
                    {
                        checkout.PropertyID,
                        checkout.CheckIn,
                        checkout.CheckOut,
                        checkout.StripeNightProductID,
                        checkout.StripeNightDefaultPriceID,
                        checkout.StripeNightDefaultUnitPrice,
                        checkout.StripeNightCouponID,
                        checkout.StripeNightPercentage,
                        checkout.OverallPrice
                    },
                    Session.Transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);

                await connection.ExecuteAsync(command);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occurred upserting a checkout with the procedure {procedure}", ex);
            }
        }
    }
}
