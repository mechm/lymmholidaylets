using LymmHolidayLets.Domain.Model.SiteMap.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IDapperSiteMapRepository : IDapperRepository<SiteMap>
    {
        SiteMap? GetById(int id);
        IEnumerable<SiteMap> GetAll();
        void Create(SiteMap siteMap);
        void Update(SiteMap siteMap);
        void Delete(int id);
    }
}
