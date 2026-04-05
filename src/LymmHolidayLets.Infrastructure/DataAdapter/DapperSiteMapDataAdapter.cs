using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperSiteMapDataAdapter(DbSession session) : SqlQueryBase(session), IDapperSiteMapDataAdapter
    {
        public bool SiteMapItemExists(string url)
        {
            const string procedure = "SiteMap_Url_Exists";

            try
            {
                using var sqlConnection = Session.Connection;
                var siteMapExists = sqlConnection.ExecuteScalar<bool>(procedure, new
                {
                    url
                },
                commandType: CommandType.StoredProcedure);               

                return siteMapExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding whether a site map item exists with the procedure {procedure}", ex);
            }
        }

        public bool SiteMapItemExistsExcludingUrl(string url, int siteMapId)
        {
            const string procedure = "SiteMap_Url_Exists_ExcludeUrl";

            try
            {
                using var sqlConnection = Session.Connection;
                var siteMapExists = sqlConnection.ExecuteScalar<bool>(procedure, new
                {
                    url,
                    siteMapId
                },
                commandType: CommandType.StoredProcedure);
               
                return siteMapExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding whether a site map item exists exluding current site map item with the procedure {procedure}", ex);
            }
        }
    }
}
