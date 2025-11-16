using Dapper;
using LymmHolidayLets.Domain.Model.SiteMap.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperSiteMapRepository : RepositoryBase<SiteMap>, IDapperSiteMapRepository
    {
        public DapperSiteMapRepository(DbSession session) : base(session) 
        {        
        }

        public IEnumerable<SiteMap> GetAll()
        {
            const string procedure = "SiteMap_GetAll";

            try
            {
                IList<SiteMap> siteMaps = new List<SiteMap>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);

                foreach (var sitemap in results)
                {
                    siteMaps.Add(new SiteMap(sitemap.SiteMapId, sitemap.Url));
                }                

                return siteMaps;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all site maps with the procedure {procedure}", ex);
            }
        }

        public SiteMap? GetById(int id)
        {
            const string procedure = "SiteMap_GetById";

            try
            {
                SiteMap sitemap;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    SiteMapId = id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                sitemap = new SiteMap(result.SiteMapId, result.Url);

                return sitemap;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a sitemap with the procedure {procedure}", ex);
            }
        }

        public void Create(SiteMap sitemap)
        {
            const string procedure = "SiteMap_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    sitemap.Url
                },
                commandType: CommandType.StoredProcedure);               
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a sitemap with the procedure {procedure}", ex);
            }
        }

        public void Update(SiteMap siteMap)
        {
            const string procedure = "SiteMap_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    siteMap.SiteMapId,
                    siteMap.Url
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a sitemap with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "SiteMap_Delete";

            try
            {
                using var connection = Session.Connection;                
                connection.Execute(procedure, new
                {
                    SiteMapId = id
                }, commandType: CommandType.StoredProcedure);
                
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a sitemap with the procedure {procedure}", ex);
            }
        }
    }
}
