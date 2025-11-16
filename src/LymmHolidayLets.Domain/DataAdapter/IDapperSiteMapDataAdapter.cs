using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperSiteMapDataAdapter : IDapperSqlQueryBase
    {
        bool SiteMapItemExists(string url);
        bool SiteMapItemExistsExcludingUrl(string url, int siteMapId);
    }
}