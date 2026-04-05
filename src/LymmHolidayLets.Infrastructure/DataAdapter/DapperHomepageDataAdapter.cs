using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Homepage;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperHomepageDataAdapter(DbSession session) : SqlQueryBase(session), IDapperHomepageDataAdapter
    {
        public HomepageAggregate GetHomePageDetail()
        {
            const string procedure = "Homepage_GetAll";

            try
            {
                using var sqlConnection = Session.Connection;
                using var result = sqlConnection.QueryMultiple(procedure, null, Session.Transaction,
                    commandType: CommandType.StoredProcedure);
                var review = result.Read<Review>();
                var slide = result.Read<Slideshow>();
                var homepageAggregate = new HomepageAggregate(review, slide);
                return homepageAggregate;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding homepage detail with the procedure {procedure}", ex);
            }
        }

        public async Task<HomepageAggregate> GetHomePageDetailAsync()
        {
            const string procedure = "Homepage_GetAll";

            try
            {
                using var sqlConnection = Session.Connection;
                await using var result = await sqlConnection.QueryMultipleAsync(procedure, null, Session.Transaction,
                  commandType: CommandType.StoredProcedure);

                var review = await result.ReadAsync<Review>();
                var slide = await result.ReadAsync<Slideshow>();
                return new HomepageAggregate(review, slide);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding homepage detail with the procedure {procedure}", ex);
            }
        }
    }
}
