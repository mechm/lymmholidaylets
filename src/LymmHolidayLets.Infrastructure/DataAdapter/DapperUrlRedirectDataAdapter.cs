using Dapper;
using LymmHolidayLets.Domain.Model.UrlRedirect.ValueType;
using LymmHolidayLets.Infrastructure;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public sealed class DapperUrlRedirectDataAdapter : SqlQueryBase, IDapperUrlRedirectDataAdapter
    {
        public DapperUrlRedirectDataAdapter(DbSession session) : base(session)
        {
        }

        public IEnumerable<UrlRedirect> GetAll()
        {
            const string procedure = "UrlRedirect_GetAll";

            try
            {
                using var connection = _session.Connection;
                var urlRedirects = connection.Query<UrlRedirect>(procedure,
                    commandType: CommandType.StoredProcedure);

                return urlRedirects;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding url redirects with the procedure {procedure}", ex);
            }
        }
    }
}
