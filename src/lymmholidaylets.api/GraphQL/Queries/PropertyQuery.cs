using LymmHolidayLets.Domain.Model.Property.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Api.GraphQL.Queries;

[ExtendObjectType("Query")]
public sealed class PropertyQuery
{
    // Query that returns a single PropertyEF via EF IQueryable to allow projection/filtering in HotChocolate
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    public IQueryable<PropertyEF> GetPropertyById(
        byte id,
        [Service] IPropertyRepositoryEF propertyRepositoryEf)
    {
        // Prefer EF repository for IQueryable (HotChocolate expects IQueryable)
        return propertyRepositoryEf.GetPropertyById(id);
    }
}
