using LymmHolidayLets.Domain.Model.FAQ.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IFAQQuery
    {
        FAQ? GetById(int id);
        IEnumerable<FAQ> GetAll();
    }
}