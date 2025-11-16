using Dapper;
using LymmHolidayLets.Domain.Model.Checkout.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
	public sealed class DapperCheckoutRepository : RepositoryBase<Checkout>, IDapperCheckoutRepository
	{
		public DapperCheckoutRepository(DbSession session) : base(session)
		{
		}

		public void Create(Checkout checkout)
		{
			const string procedure = "Checkout_Insert";

			try
			{
				using var connection = Session.Connection;
				connection.Execute(procedure, new
				{
					checkout.PropertyID,
					checkout.CheckIn,
					checkout.CheckOut,
					checkout.StripeNightProductID,
					checkout.StripeNightDefaultPriceID,
					checkout.StripeNightDefaultUnitPrice,
					checkout.StripeNightCouponID,
					checkout.StripeNightPercentage,
					checkout.OverallPrice,
					checkout.Created
				},
				Session.Transaction,
				commandType: CommandType.StoredProcedure);
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException($"An error occurred creating a checkout with the procedure {procedure}", ex);
			}
		}

		public void Update(Checkout checkout)
		{
			const string procedure = "Checkout_Update";

			try
			{
				using var connection = Session.Connection;
				connection.Execute(procedure, new
				{
					checkout.ID,
					checkout.PropertyID,
					checkout.CheckIn,
					checkout.CheckOut,
					checkout.StripeNightProductID,
					checkout.StripeNightDefaultPriceID,
					checkout.StripeNightDefaultUnitPrice,
					checkout.StripeNightCouponID,
					checkout.StripeNightPercentage,
					checkout.OverallPrice,
					checkout.Updated
				},
				Session.Transaction,
				commandType: CommandType.StoredProcedure);
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException($"An error occurred updating a checkout with the procedure {procedure}", ex);
			}
		}
	}
}
