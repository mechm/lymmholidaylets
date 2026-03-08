using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.ICal.Entity;
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

        /// <summary>
        /// Returns the next midnight UTC as an absolute cache expiration.
        /// This ensures the cached .ics file is always regenerated at a day boundary,
        /// aligning with the stored procedure which shifts stale start dates relative to today.
        /// The TTL is a safety-net only — the primary freshness mechanism is explicit cache
        /// invalidation via cache.Remove(cacheKey) when a booking is created or cancelled.
        /// </summary>
        private static DateTimeOffset NextMidnightUtc()
            => DateTime.UtcNow.Date.AddDays(1);

        /// <summary>
        /// Generates and returns an iCalendar (.ics) file for the given property,
        /// validated against the supplied access identifier.
        /// The generated calendar is cached until midnight UTC to avoid repeated generation.
        /// </summary>
        /// <param name="propertyId">The property ID (1–255).</param>
        /// <param name="identifier">The access identifier that must match the stored iCal record.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the .ics content, or <c>null</c> if invalid.</returns>
        public async Task<FileContentResult?> GetCalendarAsync(
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
            if (!ical.Any(i => i.PropertyID == propertyId && i.Identifier == identifier))
            {
                logger.LogWarning(
                    "No matching calendar for PropertyId={PropertyId}, Identifier={Identifier}",
                    propertyId, identifier);
                return null;
            }

            var cacheKey = GetAvailabilityCacheKey(propertyId);
            if (!cache.TryGetValue(cacheKey, out string? calendar))
            {
                try
                {
                    calendar = await icalGenerator.GenerateCalendarAsync(propertyId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to generate calendar for PropertyId={PropertyId}", propertyId);
                    return null;
                }

                // Cache the generated .ics for a short period to avoid regenerating on every request
                // while still reflecting new bookings reasonably quickly.
                // Trade-off: a longer TTL reduces DB load but increases the window in which an
                // external calendar client (Google Calendar, Apple Calendar etc.) could display
                // stale availability after a new booking is made.
                cache.Set(cacheKey, calendar, NextMidnightUtc());
            }

            if (string.IsNullOrEmpty(calendar))
            {
                logger.LogError("Generated calendar is empty for PropertyId={PropertyId}", propertyId);
                return null;
            }

            // RFC 5545 mandates UTF-8 encoding for iCalendar data.
            var bytes = Encoding.UTF8.GetBytes(calendar);
            return new FileContentResult(bytes, "text/calendar; charset=utf-8")
            {
                FileDownloadName = $"{propertyId}.ics"
            };
        }

        private async Task<IReadOnlyList<ICal>> GetCachedIcalResultsAsync(CancellationToken cancellationToken)
        {
            return await cache.GetOrCreateAsync<IReadOnlyList<ICal>>(IcalResultsCacheKey, async entry =>
            {
                logger.LogInformation("iCal metadata cache miss. Fetching from database.");
                entry.Priority = CacheItemPriority.Normal;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                return await icalQuery.GetAllAsync(cancellationToken);
            }) ?? [];
        }

        private static string GetAvailabilityCacheKey(byte propertyId)
            => $"ical-availability-{propertyId}";
    }
}
