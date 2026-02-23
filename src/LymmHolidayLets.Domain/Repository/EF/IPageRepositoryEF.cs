using LymmHolidayLets.Domain.Model.Page.Entity;

namespace LymmHolidayLets.Domain.Repository.EF
{
    public interface IPageRepositoryEF
    {
        IQueryable<PageEF> GetPageById(int id);
    }
}
