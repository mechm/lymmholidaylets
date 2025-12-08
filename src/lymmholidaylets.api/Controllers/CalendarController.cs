using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace lymmholidaylets.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CalendarController(IMemoryCache cache, ICalQuery icalQuery) : ControllerBase
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ICalQuery _icalQuery = icalQuery;

        // GET api/calendar/ical/1.ics?s={guid}
        [HttpGet("ical/{id:int:min(1)}.ics")]
        public IActionResult ICal(int id, [FromQuery] Guid s)
        {
            if (id < 1 || id > byte.MaxValue) return BadRequest();

            if (!_cache.TryGetValue("ical-results", out IList<ICal>? ical))
            {
                ical = _icalQuery.GetAll();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.Normal)
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                _cache.Set("ical-results", ical, cacheEntryOptions);
            }

            if (ical == null || !ical.Any(i => i.PropertyID == (byte)id && i.Identifier == s))
            {
                return BadRequest();
            }

            var cacheKey = $"ical-availability-{id}";
            if (!_cache.TryGetValue(cacheKey, out string? calendar))
            {
                calendar = GetCalendar((byte)id);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.Normal)
                    .SetAbsoluteExpiration(new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1)));

                _cache.Set(cacheKey, calendar, cacheEntryOptions);
            }

            if (!string.IsNullOrEmpty(calendar))
            {
                var bytes = Encoding.ASCII.GetBytes(calendar);
                return File(bytes, "text/calendar", $"{id}.ics");
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        private string GetCalendar(byte id)
        {
            var availability = _icalQuery.GetICalAvailability(id);

            const string productId = "-//LymmHolidayLets//Hosting Calendar//EN";
            const string defaultProductId = "-//github.com/rianjs/ical.net//NONSGML ical.net 4.0//EN";

            var calendar = new Calendar
            {
                Scale = "GREGORIAN",
                Version = "2.0"
            };

            foreach (AvailabilityICal available in availability)
            {
                var icalEvent = new CalendarEvent
                {
                    DtStart = new CalDateTime(available.StartDate),
                    DtEnd = new CalDateTime(available.EndDate),
                    Uid = StringToGUID($"{available.BookingID}-{available.StartDate.Date}-{available.EndDate.Date}").ToString(),
                    Location = $"{available.FriendlyName}({id})",
                    Summary = available.BookingID != null ? "Reserved" : "CLOSED - Not available"
                };

                if (available.BookingID != null)
                {
                    icalEvent.Description = $"Reservation URL: https://lymmholidaylets.com/property/detail/{id} details/{id}\n" +
                                            $"Phone Number (Last 4 Digits): {available.LastFourDigitTelephone}\n" +
                                            $"Check in: {available.StartDate}\n" +
                                            $"Check out: {available.EndDate}\n" +
                                            $"Nights: {available.NoOfNights}\n" +
                                            $"Guests: {available.NoOfGuests}";
                }

                calendar.Events.Add(icalEvent);
            }

            var serializer = new CalendarSerializer(calendar);
            string? result = serializer.SerializeToString() ?? throw new InvalidOperationException("Failed to serialize calendar to string.");
            // https://github.com/rianjs/ical.net/issues/408
            result = result.Replace(defaultProductId, productId);

            return result;
        }

        private static Guid StringToGUID(string value)
        {
            byte[] data = MD5.HashData(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }
    }
}
