using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using LymmHolidayLets.Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace LymmHolidayLets.Api.Services
{
    public sealed class CalService(
        IMemoryCache cache,
        ICalQuery icalQuery,
        ICalGenerator icalGenerator,
        ILogger<CalService> logger) : ICalService
    {
        private const string IcalResultsCacheKey = "ical-results";

        public async Task<FileContentResult?> GetCalendarAsync(int id, Guid s)
        {
            if (id is < 1 or > byte.MaxValue)
            {
                logger.LogWarning("Invalid property ID requested: {PropertyId}", id);
                return null;
            }

            var ical = await GetCachedIcalResultsAsync();
            if (!ical.Any(i => i.PropertyID == (byte)id && i.Identifier == s))
            {
                logger.LogWarning("No matching calendar for PropertyId={PropertyId}, Session={Session}", id, s);
                return null;
            }

            var cacheKey = GetAvailabilityCacheKey(id, s);
            if (!cache.TryGetValue(cacheKey, out string? calendar))
            {
                try
                {
                    calendar = await icalGenerator.GenerateCalendarAsync((byte)id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to generate calendar for PropertyId={PropertyId}", id);
                    return null;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.Normal)
                    .SetAbsoluteExpiration(DateTimeOffset.UtcNow.Date.AddDays(1).AddSeconds(-1));
                cache.Set(cacheKey, calendar, cacheEntryOptions);
            }

            if (string.IsNullOrEmpty(calendar))
            {
                logger.LogError("Generated calendar is empty for PropertyId={PropertyId}", id);
                return null;
            }

            var bytes = Encoding.ASCII.GetBytes(calendar);
            return new FileContentResult(bytes, "text/calendar") { FileDownloadName = $"{id}.ics" };
        }

        private async Task<IReadOnlyList<ICal>> GetCachedIcalResultsAsync()
        {
            return await cache.GetOrCreateAsync<IReadOnlyList<ICal>>(IcalResultsCacheKey, async entry =>
            {
                logger.LogInformation("iCal metadata cache miss. Fetching from database.");
                entry.Priority = CacheItemPriority.Normal;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                return await icalQuery.GetAllAsync();
            }) ?? new List<ICal>();
        }

        private static string GetAvailabilityCacheKey(int id, Guid s) => $"ical-availability-{id}-{s}";
    }
}
