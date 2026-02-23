using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Application.Interface.Query;

namespace LymmHolidayLets.Api.GraphQL.Queries;

[ExtendObjectType("Query")]
public sealed class PageQuery
{
    [UseSingleOrDefault] // 3. Finally, grab the one item from the projected result
    [UseProjection]      // 2. Second, look at the GraphQL query and select only columns
    public IQueryable<PageEF> GetPageById(
        int id,
        [Service] IPageQuery query)
    {
        // Call the application layer IPageQuery instead of wiring directly to EF repository
        return query.GetPageByIdEf(id);
    }
}