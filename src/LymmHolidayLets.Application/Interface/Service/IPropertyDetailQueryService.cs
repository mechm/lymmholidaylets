using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Application.Interface.Service;

public interface IPropertyDetailQueryService
{
    Task<PropertyDetailResult?> GetPropertyDetailAsync(byte propertyId, CancellationToken cancellationToken = default);
}
