using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class CalendarRepositoryEF(AppDbContext context) : ICalendarRepositoryEF
    {
        private readonly AppDbContext _context = context;

        public IQueryable<CalendarEF> GetCalendarByIdAsync(int id)
        {
            return _context.Calendar.Where(x => x.ID == id).OrderBy(x => x.ID);;
        }
    }
}
