using LymmHolidayLets.Domain.Model.Calendar.Entity;


namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class CalendarRepositoryEF //: IBookRepository
    {
        private readonly AppDbContext _context;

        public CalendarRepositoryEF(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Calendar?> GetCalendarByIdAsync(int id)
        {
            return await _context.Calendars.FindAsync(id);
        }
    }
}
