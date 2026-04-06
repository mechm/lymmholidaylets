using LymmHolidayLets.Application.Interface.Service;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Services
{
    public sealed class PropertyCacheInvalidator(IMemoryCache cache) : IPropertyCacheInvalidator
    {
        public void Invalidate(byte propertyId) =>
            cache.Remove($"property-detail-{propertyId}");
    }
}
