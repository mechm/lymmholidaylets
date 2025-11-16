using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Homepage;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperHomepageDataAdapter : SqlQueryBase, IDapperHomepageDataAdapter
    {
        public DapperHomepageDataAdapter(DbSession session) : base(session)
        {
        }

        public HomepageAggregate GetHomePageDetail()
        {
            const string procedure = "Homepage_GetAll";

            try
            {
                HomepageAggregate homepageAggregate;

                using var sqlConnection = _session.Connection;
                using (var result = sqlConnection.QueryMultiple(procedure, null, _session.Transaction,
                  commandType: CommandType.StoredProcedure))
                {
                    IEnumerable<Review> review = result.Read<Review>();
                    IEnumerable<Slideshow> slide = result.Read<Slideshow>();
                    homepageAggregate = new HomepageAggregate(review, slide);
                }
                return homepageAggregate;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding homepage detail with the procedure {procedure}", ex);
            }
        }
    }
}
