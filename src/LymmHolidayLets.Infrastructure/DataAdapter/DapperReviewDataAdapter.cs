using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperReviewDataAdapter(DbSession session) : SqlQueryBase(session), IDapperReviewDataAdapter
    {
        public IReadOnlyList<ReviewSummary> GetAllApprovedReviews()
        {
            const string procedure = "Review_Summaries";

            try
            {
                using var connection = Session.Connection;
                return connection.Query<ReviewSummary>(procedure, new
                {
                    approved = true
                }, Session.Transaction,
                commandType: CommandType.StoredProcedure).AsList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding reviews with the procedure {procedure}", ex);
            }
        }

        public async Task<IReadOnlyList<ReviewSummary>> GetAllApprovedReviewsAsync()
        {
            const string procedure = "Review_Summaries";

            try
            {
                using var connection = Session.Connection;
                var results = await connection.QueryAsync<ReviewSummary>(procedure, new
                {
                    approved = true
                }, Session.Transaction,
                commandType: CommandType.StoredProcedure);
                return results.AsList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding reviews with the procedure {procedure}", ex);
            }
        }
    }
}
