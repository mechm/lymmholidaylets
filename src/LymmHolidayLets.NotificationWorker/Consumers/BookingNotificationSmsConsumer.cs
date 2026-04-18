using LymmHolidayLets.Contracts;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using MassTransit;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LymmHolidayLets.NotificationWorker.Consumers;

/// <summary>
/// Sends SMS notifications when a booking is confirmed.
/// Consumes BookingNotificationRequested and sends a text message to configured recipients.
/// Intentionally separate from email consumers so failures are independent.
/// </summary>
public sealed class BookingNotificationSmsConsumer(
    ITextMessageService textMessageService,
    IOptions<SmsNotificationOptions> smsOptions,
    ILogger<BookingNotificationSmsConsumer> logger) : IConsumer<BookingNotificationRequested>
{
    public async Task Consume(ConsumeContext<BookingNotificationRequested> context)
    {
        var evt = context.Message;
        var recipients = smsOptions.Value.Recipients
            .Where(number => !string.IsNullOrWhiteSpace(number))
            .ToArray();

        logger.LogInformation(
            "Sending SMS notification for booking: {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);

        if (!smsOptions.Value.Enabled)
        {
            logger.LogInformation(
                "SMS notification disabled for booking: {PropertyName}, Guest={GuestName}",
                evt.PropertyName, evt.Name);
            return;
        }

        if (recipients.Length == 0)
        {
            logger.LogWarning("No SMS recipients configured for booking notification");
            return;
        }

        decimal? amount = evt.AmountTotal / 100M;
        string price = amount.HasValue
            ? amount.Value.ToString(amount.Value % 1 > 0 ? "C2" : "C0", CultureInfo.GetCultureInfo("en-GB"))
            : "N/A";
        var nights = evt.CheckOut.DayNumber - evt.CheckIn.DayNumber;
        var nightsText = nights == 1 ? "1 night" : $"{nights} nights";
        var guestSummary = FormatGuestSummary(evt.NoAdult, evt.NoChildren, evt.NoInfant);
        var bookingReference = string.IsNullOrWhiteSpace(evt.BookingReference) ? "N/A" : evt.BookingReference;
        var telephone = string.IsNullOrWhiteSpace(evt.Telephone) ? "Not provided" : evt.Telephone;

        string message = $"New booking: {evt.Name} booked {evt.PropertyName} ({nightsText}) {evt.CheckIn:dd/MM/yyyy}-{evt.CheckOut:dd/MM/yyyy} " +
                         $"for {price}. {guestSummary}. Ref: {bookingReference}. Tel: {telephone}";

        await textMessageService.SendText(message, recipients);

        logger.LogInformation(
            "SMS notification sent for booking: {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);
    }

    private static string FormatGuestSummary(byte? noAdult, byte? noChildren, byte? noInfant)
    {
        var parts = new List<string>();

        AddGuestPart(parts, noAdult, "adult");
        AddGuestPart(parts, noChildren, "child");
        AddGuestPart(parts, noInfant, "infant");

        return parts.Count == 0 ? "Guests not provided" : string.Join(", ", parts);
    }

    private static void AddGuestPart(List<string> parts, byte? count, string label)
    {
        if (count is not > 0)
        {
            return;
        }

        parts.Add(count.Value == 1 ? $"1 {label}" : $"{count.Value} {label}s");
    }
}
