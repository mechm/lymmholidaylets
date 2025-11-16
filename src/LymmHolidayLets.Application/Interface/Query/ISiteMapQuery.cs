using LymmHolidayLets.Domain.Model.SiteMap.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface ISiteMapQuery
    {
        SiteMap? GetById(int id);
        IEnumerable<SiteMap> GetAll();
        bool SiteMapItemExists(string url);
        bool SiteMapItemExistsExcludingUrl(string url, int siteMapId);
    }
}