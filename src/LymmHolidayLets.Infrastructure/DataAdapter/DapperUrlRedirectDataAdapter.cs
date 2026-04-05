using System.Data;
using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.UrlRedirect.ValueType;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperUrlRedirectDataAdapter(DbSession session)
        : SqlQueryBase(session), IDapperUrlRedirectDataAdapter
    {
        public IEnumerable<UrlRedirect> GetAll()
        {
            const string procedure = "UrlRedirect_GetAll";

            try
            {
                using var connection = Session.Connection;
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
