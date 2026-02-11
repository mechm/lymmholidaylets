using HotChocolate;
using HotChocolate.Data;

using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Calendar.Entity;

namespace LymmHolidayLets.Api.GraphQL.Queries;
public sealed class CalendarQuery
{

    // The query entry point for fetching a single calendar item by ID.
    // This method is called by the GraphQL execution engine.
    [UseFirstOrDefault] // ⬅️ Tells HotChocolate to execute with .FirstOrDefaultAsync()
    [UseProjection]     // ⬅️ Tells HotChocolate to add a .Select() for optimization
    [UseFiltering]      // ⬅️ Optional: Adds filtering capabilities
    public IQueryable<CalendarEF> GetCalendarById(
        int id,
        [Service] ICalendarQuery query) // Inject the repository
    {
        return query.GetCalendarById(id); 
    }
}
