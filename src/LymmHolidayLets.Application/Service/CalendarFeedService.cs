using System.Text;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service;

public sealed class CalendarFeedService(
    IApplicationCache cache,
    ICalQuery icalQuery,
    ICalGenerator icalGenerator,
    ILogger<CalendarFeedService> logger) : ICalendarFeedService
{
    private const string IcalResultsCacheKey = "ical-results";

    public async Task<CalendarFeedResult?> GetCalendarAsync(
        byte propertyId,
        Guid identifier,
        CancellationToken cancellationToken = default)
    {
        if (propertyId < 1)
        {
            logger.LogWarning("Invalid property ID requested: {PropertyId}", propertyId);
            return null;
        }

        var ical = await GetCachedIcalResultsAsync(cancellationToken);
        if (!ical.Any(item => item.PropertyID == propertyId && item.Identifier == identifier))
        {
            logger.LogWarning(
                "No matching calendar for PropertyId={PropertyId}, Identifier={Identifier}",
                propertyId,
                identifier);
            return null;
        }

        var cacheKey = GetAvailabilityCacheKey(propertyId);
        if (!cache.TryGetValue(cacheKey, out string? calendar))
        {
            try
            {
                calendar = await icalGenerator.GenerateCalendarAsync(propertyId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate calendar for PropertyId={PropertyId}", propertyId);
                return null;
            }

            cache.SetAbsolute(cacheKey, calendar, NextMidnightUtc());
        }

        if (!string.IsNullOrEmpty(calendar))
        {
            return new CalendarFeedResult
            {
                FileContents = Encoding.UTF8.GetBytes(calendar),
                ContentType = "text/calendar; charset=utf-8",
                FileDownloadName = $"{propertyId}.ics"
            };
        }

        logger.LogError("Generated calendar is empty for PropertyId={PropertyId}", propertyId);
        return null;

    }

    private async Task<IReadOnlyList<ICal>> GetCachedIcalResultsAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(IcalResultsCacheKey, out IReadOnlyList<ICal>? icalResults) && icalResults is not null)
        {
            return icalResults;
        }

        logger.LogInformation("iCal metadata cache miss. Fetching from database.");
        icalResults = await icalQuery.GetAllAsync(cancellationToken);
        cache.SetAbsolute(IcalResultsCacheKey, icalResults, TimeSpan.FromHours(24));
        return icalResults;
    }

    private static DateTimeOffset NextMidnightUtc() => DateTime.UtcNow.Date.AddDays(1);

    private static string GetAvailabilityCacheKey(byte propertyId) => $"ical-availability-{propertyId}";
}
