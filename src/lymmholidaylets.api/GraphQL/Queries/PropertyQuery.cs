using LymmHolidayLets.Domain.Model.Property.Entity;
using LymmHolidayLets.Application.Interface.Query;

namespace LymmHolidayLets.Api.GraphQL.Queries;

[ExtendObjectType("Query")]
public sealed class PropertyQuery
{
    [UseSingleOrDefault] // 3. Finally, grab the one item from the projected result
    [UseProjection]      // 2. Second, look at the GraphQL query and select only columns
    public IQueryable<PropertyEF> GetPropertyById(
        byte id,
        [Service] IPropertyQuery query)
    {
        return query.GetPropertyByIdEf(id);
    }
}
