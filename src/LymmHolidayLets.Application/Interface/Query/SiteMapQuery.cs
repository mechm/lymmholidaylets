using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.SiteMap.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Interface.Query
{
    public sealed class SiteMapQuery : ISiteMapQuery
    {
        private readonly IDapperSiteMapRepository _siteMapRepository;
        private readonly IDapperSiteMapDataAdapter _siteMapDataAdapter;

        public SiteMapQuery(IDapperSiteMapRepository siteMapRepository,
            IDapperSiteMapDataAdapter siteMapDataAdapter)
        {
            _siteMapRepository = siteMapRepository;
            _siteMapDataAdapter = siteMapDataAdapter;
        }

        public SiteMap? GetById(int id)
        {
            return _siteMapRepository.GetById(id);
        }

        public IEnumerable<SiteMap> GetAll()
        {
            return _siteMapRepository.GetAll();
        }

        public bool SiteMapItemExists(string url)
        {
            return _siteMapDataAdapter.SiteMapItemExists(url);
        }

        public bool SiteMapItemExistsExcludingUrl(string url, int siteMapId)
        {
            return _siteMapDataAdapter.SiteMapItemExistsExcludingUrl(url, siteMapId);
        }
    }
}
