using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Calendar.Entity;

namespace LymmHolidayLets.Api.GraphQL.Queries;

[ExtendObjectType("Query")]
public sealed class CalendarQuery
{    
    [UseSingleOrDefault] // 3. Finally, grab the one item from the projected result
    [UseProjection]      // 2. Second, look at the GraphQL query and select only columns
    public IQueryable<CalendarEF> GetCalendarById(
        int id,
        [Service] ICalendarQuery query) // Inject the repository
    {
        return query.GetCalendarById(id); 
    }
}
