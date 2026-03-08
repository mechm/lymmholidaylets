using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class SiteMapCommand(ISiteMapRepository siteMapRepository) : ISiteMapCommand
    {
        public void Create(SiteMap siteMap)
        {
            siteMapRepository.Create(
               new Domain.Model.SiteMap.Entity.SiteMap(siteMap.Url));
        }

        public void Update(SiteMap siteMap)
        {
            siteMapRepository.Update(
                new Domain.Model.SiteMap.Entity.SiteMap(siteMap.SiteMapId, siteMap.Url));
        }

        public void Delete(int id)
        {
            siteMapRepository.Delete(id);
        }
    }
}
