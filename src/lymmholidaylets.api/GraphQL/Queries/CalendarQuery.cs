using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Dto;
using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Infrastructure;

namespace lymmholidaylets.api.GraphQL.Queries
{
    public sealed class CalendarQuery(ICalendarQuery calendarQuery)
    {
        private readonly ICalendarQuery _calendarQuery = calendarQuery;

        //[UseDbContext(typeof(AppDbContext))]
        //[UseProjection]     // Enables EF-level SELECT projection
        //[UseFiltering]
        //[UseSorting]
        //public IQueryable<LymmHolidayLets.Domain.Model.Calendar.Entity.CalendarEF> GetCalendarById(int id) => _calendarQuery.GetCalendarById(id);

        [UseProjection]
        public IQueryable<CalendarEF> Calendar(int id, [Service] ICalendarQuery query)
    => query.GetCalendarById(id);
        //Task<CalendarDto?> GetCalendarById(int id) =>  _calendarQuery.GetCalendarById(id);
    }
}
