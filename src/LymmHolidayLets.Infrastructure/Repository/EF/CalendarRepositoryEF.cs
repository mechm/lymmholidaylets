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

        //public async Task<CalendarEF?> GetCalendarByIdAsync(int id)
        //{
        //    // Execute the query immediately while the DbContext is alive.
        //    // Use FindAsync if you want PK lookup (returns entity or null).
        //    return await _context.Calendar.FindAsync(id);
        //    // Alternatively, if you prefer LINQ:
        //    // return await _context.Calendar.SingleOrDefaultAsync(x => x.ID == id);
        //}
    }
}
