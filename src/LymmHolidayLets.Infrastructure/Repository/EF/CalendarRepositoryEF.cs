using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class CalendarRepositoryEF(AppDbContext context) : ICalendarRepositoryEF
    {
        private readonly AppDbContext _context = context;

        public IQueryable<CalendarEF> GetCalendarByIdAsync(int id)
        {
            return _context.Calendar.Where(x => x.ID == id);
        }

        /// <summary>
        /// Returns the raw IQueryable, allowing HotChocolate to apply filtering and projection.
        /// Execution is deferred!
        /// </summary>
        public IQueryable<CalendarEF> GetBaseQuery()
        {
            // 🚨 IMPORTANT: NO .Where(), NO .ToList(), NO .FirstOrDefaultAsync()
            return _context.Calendar;
        }
    }
}
