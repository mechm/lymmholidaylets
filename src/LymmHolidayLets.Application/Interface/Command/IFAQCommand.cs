using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IFAQCommand
    {
        void Create(FAQ faq);
        void Update(FAQ faq);
        void Delete(int id);
    }
}