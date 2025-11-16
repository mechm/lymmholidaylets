using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface ISiteMapCommand
    {
        void Create(SiteMap siteMap);
        void Update(SiteMap siteMap);
        void Delete(int id);
    }
}