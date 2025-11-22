using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class CalendarRepositoryEF : ICalendarRepositoryEF
    {
        private readonly AppDbContext _context;

        public CalendarRepositoryEF(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<CalendarEF> GetCalendarByIdAsync(int id)
        {
            return _context.Calendar.Where(x => x.ID == id);//.FindAsync(id);
            //return _context.Calendar.FindAsync(id);
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
