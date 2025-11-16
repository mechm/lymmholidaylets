using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class SiteMapCommand : ISiteMapCommand
    {
        private readonly IDapperSiteMapRepository _siteMapRepository;

        public SiteMapCommand(IDapperSiteMapRepository siteMapRepository)
        {
            _siteMapRepository = siteMapRepository;
        }

        public void Create(SiteMap siteMap)
        {
            _siteMapRepository.Create(
               new Domain.Model.SiteMap.Entity.SiteMap(siteMap.Url));
        }

        public void Update(SiteMap siteMap)
        {
            _siteMapRepository.Update(
                new Domain.Model.SiteMap.Entity.SiteMap(siteMap.SiteMapId, siteMap.Url));
        }

        public void Delete(int id)
        {
            _siteMapRepository.Delete(id);
        }
    }
}