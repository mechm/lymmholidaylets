using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Property;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service;

public sealed class PropertyDetailQueryService(
    IApplicationCache cache,
    IPropertyQuery propertyQuery,
    ILogger<PropertyDetailQueryService> logger) : IPropertyDetailQueryService
{
    public async Task<PropertyDetailResult?> GetPropertyDetailAsync(byte propertyId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"property-detail-{propertyId}";

        if (cache.TryGetValue(cacheKey, out PropertyDetailResult? detail))
        {
            var currentCalendarTs = await propertyQuery.GetCalendarLastModifiedAsync(propertyId);
            if (currentCalendarTs != detail?.CalendarLastModified)
            {
                logger.LogInformation(
                    "Calendar availability changed for PropertyId={PropertyId}, evicting cache entry.",
                    propertyId);
                cache.Remove(cacheKey);
                detail = null;
            }
        }

        if (detail is not null)
        {
            return detail;
        }

        logger.LogInformation(
            "Property detail cache miss for PropertyId={PropertyId}. Fetching from database.",
            propertyId);

        detail = await propertyQuery.GetPropertyDetailByIdAsync(propertyId);

        if (detail is not null)
        {
            cache.SetAbsolute(cacheKey, detail, TimeSpan.FromHours(1));
        }

        return detail;
    }
}
