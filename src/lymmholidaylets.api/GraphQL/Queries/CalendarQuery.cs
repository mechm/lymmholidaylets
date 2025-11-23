using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Dto;
using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Infrastructure;

namespace lymmholidaylets.api.GraphQL.Queries
{
    public sealed class CalendarQuery
    {
        //private readonly ICalendarQuery _calendarQuery = calendarQuery;

        //[UseDbContext(typeof(AppDbContext))]
       // [UseProjection]     // Enables EF-level SELECT projection
        //[UseFiltering]
        //[UseSorting]
        //public IQueryable<LymmHolidayLets.Domain.Model.Calendar.Entity.CalendarEF> GetCalendarById(int id) => _calendarQuery.GetCalendarById(id);

       // [UseProjection]
        //public IQueryable<CalendarEF> Calendar(int id, [Service] ICalendarQuery query) => query.GetCalendarById(id);


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
            // 1. Get the base IQueryable from the repository.
            //IQueryable<CalendarEF> cal = query.GetCalendarById(id);

            // 2. Apply your specific filtering logic (get by ID).
            // This query is still NOT executed.
            //return cal.Where(c => c.ID == id);
        }


        //Task<CalendarDto?> GetCalendarById(int id) =>  _calendarQuery.GetCalendarById(id);
    }
}
