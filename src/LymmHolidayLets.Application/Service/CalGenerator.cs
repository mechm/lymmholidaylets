using LymmHolidayLets.Application.Interface.Query;
using System.Security.Cryptography;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Application.Service
{
    public sealed class CalGenerator(ICalQuery icalQuery) : ICalGenerator
    {
        private const string DefaultProductId = "-//github.com/rianjs/ical.net//";
        private const string ProductId = "-//lymmholidaylets.co.uk//";

        public async Task<string> GenerateCalendarAsync(byte propertyId)
        {
            var availability = await icalQuery.GetICalAvailabilityAsync(propertyId);
            var calendar = new Calendar { Scale = "GREGORIAN", Version = "2.0" };
            calendar.AddProperty("X-WR-CALNAME", $"Lymm {propertyId}");

            foreach (var available in availability)
            {
                var icalEvent = new CalendarEvent
                {
                    DtStart = new CalDateTime(available.StartDate, "UTC"),
                    DtEnd = new CalDateTime(available.EndDate, "UTC"),
                    Uid = StringToGuid($"{available.BookingID}-{available.StartDate.Date}-{available.EndDate.Date}").ToString(),
                    Location = $"{available.FriendlyName}({propertyId})",
                    Summary = available.BookingID != null ? "Reserved" : "CLOSED - Not available"
                };

                if (available.BookingID != null)
                {
                    icalEvent.Description =
                        $"Reservation URL: https://lymmholidaylets.com/property/detail/{propertyId} details/{propertyId}\n" +
                        $"Phone Number (Last 4 Digits): {available.LastFourDigitTelephone}\n" +
                        $"Check in: {available.StartDate}\n" +
                        $"Check out: {available.EndDate}\n" +
                        $"Nights: {available.NoOfNights}\n" +
                        $"Guests: {available.NoOfGuests}";
                }

                calendar.Events.Add(icalEvent);
            }

            var serializer = new CalendarSerializer(calendar);
            var result = serializer.SerializeToString()
                ?? throw new InvalidOperationException("Failed to serialize calendar to string.");
            
            return result.Replace(DefaultProductId, ProductId);
        }

        private static Guid StringToGuid(string value)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            var guidBytes = new byte[16];
            Array.Copy(hash, 0, guidBytes, 0, 16);
            return new Guid(guidBytes);
        }
    }
}
