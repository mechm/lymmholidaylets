using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IPageCommand
    {
        void Create(Page page);
        void Update(Page page);
        void Delete(int id);
    }
}
