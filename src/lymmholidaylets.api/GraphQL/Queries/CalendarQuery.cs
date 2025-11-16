using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Dto;

namespace lymmholidaylets.api.GraphQL.Queries
{
    public sealed class CalendarQuery(ICalendarQuery calendarQuery)
    {
        private readonly ICalendarQuery _calendarQuery = calendarQuery;
           
        public CalendarDto? GetCalendarById(int id) => _calendarQuery.GetCalendarById(id);
    }
}
