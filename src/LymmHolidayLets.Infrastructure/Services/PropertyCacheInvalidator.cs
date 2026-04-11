using LymmHolidayLets.Application.Interface.Service;

namespace LymmHolidayLets.Infrastructure.Services;

public sealed class PropertyCacheInvalidator(IApplicationCache cache) : IPropertyCacheInvalidator
{
    public void Invalidate(byte propertyId) =>
        cache.Remove($"property-detail-{propertyId}");
}
