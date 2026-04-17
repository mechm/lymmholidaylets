using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;
using System.Security.Cryptography;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service
{
    public sealed class CalGenerator(ICalQuery icalQuery, ILogger<CalGenerator> logger) : ICalGenerator
    {
        private const string DefaultProductId  = "-//github.com/rianjs/ical.net//";
        private const string ProductId         = "-//lymmholidaylets.com//";
        private const string PropertyDetailUrl = "https://lymmholidaylets.com/property/detail";

        /// <summary>
        /// Generates an iCalendar string for the given property, containing one
        /// VEVENT per availability window returned by the data store.
        /// </summary>
        public async Task<string> GenerateCalendarAsync(byte propertyId, CancellationToken cancellationToken = default)
        {
            var availability = await icalQuery.GetICalAvailabilityAsync(propertyId, cancellationToken);

            if (availability.Count == 0)
            {
                logger.LogWarning("No availability records found for PropertyId={PropertyId} — generating empty calendar", propertyId);
            }

            // Use FriendlyName from first record if available; fall back to property ID.
            var calName = availability.Count > 0 ? availability[0].FriendlyName : $"Property {propertyId}";

            var calendar = new Calendar { Scale = "GREGORIAN", Version = "2.0" };
            calendar.AddProperty("X-WR-CALNAME", calName);

            foreach (var available in availability)
            {
                calendar.Events.Add(BuildCalendarEvent(available, propertyId));
            }

            var serializer = new CalendarSerializer(calendar);
            var result = serializer.SerializeToString()
                ?? throw new InvalidOperationException($"Failed to serialize calendar to string for PropertyId={propertyId}.");

            return result.Replace(DefaultProductId, ProductId);
        }

        /// <summary>
        /// Builds a single VEVENT from an availability record.
        /// Booked slots include reservation details in the description;
        /// closed/blocked slots use a minimal summary only.
        /// </summary>
        private static CalendarEvent BuildCalendarEvent(AvailabilityICal available, byte propertyId)
        {
            var icalEvent = new CalendarEvent
            {
                DtStart  = new CalDateTime(available.StartDate, "UTC"),
                DtEnd    = new CalDateTime(available.EndDate, "UTC"),
                // UID is deterministic — same booking + dates always produce the same GUID,
                // which prevents duplicate events in calendar clients on repeated syncs.
                Uid      = BuildDeterministicUid(available),
                Location = $"{available.FriendlyName} ({propertyId})",
                Summary  = available.BookingID != null ? "Reserved" : "CLOSED - Not available"
            };

            if (available.BookingID != null)
            {
                icalEvent.Description =
                    $"Reservation URL: {PropertyDetailUrl}/{propertyId}\n" +
                    $"Phone Number (Last 4 Digits): {available.LastFourDigitTelephone}\n" +
                    $"Check in: {available.StartDate:yyyy-MM-dd}\n" +
                    $"Check out: {available.EndDate:yyyy-MM-dd}\n" +
                    $"Nights: {available.NoOfNights}\n" +
                    $"Guests: {available.NoOfGuests}";
            }

            return icalEvent;
        }

        /// <summary>
        /// Produces a deterministic GUID from a booking ID and date range so that
        /// repeated calendar syncs do not create duplicate events in external clients.
        /// </summary>
        private static string BuildDeterministicUid(AvailabilityICal available)
        {
            var raw   = $"{available.BookingID}-{available.StartDate.Date}-{available.EndDate.Date}";
            var hash  = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            var bytes = new byte[16];
            Array.Copy(hash, 0, bytes, 0, 16);
            return new Guid(bytes).ToString();
        }
    }
}
