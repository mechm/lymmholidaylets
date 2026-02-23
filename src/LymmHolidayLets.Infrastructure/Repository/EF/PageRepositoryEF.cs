using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class PageRepositoryEF(AppDbContext context) : IPageRepositoryEF
    {
        private readonly AppDbContext _context = context;

        public IQueryable<PageEF> GetPageById(int id)
        {
            return _context.Page.Where(x => x.PageId == id).OrderBy(x => x.PageId);;
        }
    }
}
