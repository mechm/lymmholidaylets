using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;
using System.Data.SqlClient;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperReviewDataAdapter : SqlQueryBase, IDapperReviewDataAdapter
    {
        public DapperReviewDataAdapter(DbSession session) : base(session)
        {
        }

        public IEnumerable<ReviewSummary> GetAllApprovedReviews()
        {
            const string procedure = "Review_Summaries";

            try
            {
                using var connection = _session.Connection;
                var reviews = connection.Query<ReviewSummary>(procedure, new
                {
                    approved = true
                }, _session.Transaction,
                commandType: CommandType.StoredProcedure);

                return reviews;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding reviews with the procedure {procedure}", ex);
            }
        }
    }
}
