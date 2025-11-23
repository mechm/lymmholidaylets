using LymmHolidayLets.Domain.Model.Calendar.Entity;

namespace LymmHolidayLets.Domain.Repository.EF
{
    public interface ICalendarRepositoryEF
    {
        IQueryable<CalendarEF> GetBaseQuery();
        IQueryable<CalendarEF> GetCalendarByIdAsync(int id);
        //Task<CalendarEF?> GetCalendarByIdAsync(int id);
    }
}
