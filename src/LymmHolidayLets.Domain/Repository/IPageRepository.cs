using LymmHolidayLets.Domain.Model.Page.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IPageRepository : IRepository<Page>
    {
        Page? GetById(int id);
        IEnumerable<Page> GetAll();
        void Create(Page page);
        void Update(Page page);
        void Delete(int id);
    }
}
