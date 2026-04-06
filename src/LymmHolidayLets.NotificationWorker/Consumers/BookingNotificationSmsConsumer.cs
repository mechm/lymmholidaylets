using LymmHolidayLets.Contracts;
using LymmHolidayLets.NotificationWorker.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace LymmHolidayLets.NotificationWorker.Consumers;

/// <summary>
/// Sends SMS notifications when a booking is confirmed.
/// Consumes BookingNotificationRequested and sends a text message to configured recipients.
/// Intentionally separate from email consumers so failures are independent.
/// </summary>
public sealed class BookingNotificationSmsConsumer(
    ITextMessageService textMessageService,
    ILogger<BookingNotificationSmsConsumer> logger) : IConsumer<BookingNotificationRequested>
{
    public async Task Consume(ConsumeContext<BookingNotificationRequested> context)
    {
        var evt = context.Message;

        logger.LogInformation(
            "Sending SMS notification for booking: {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);

        if (evt.SmsRecipients.Length == 0)
        {
            logger.LogWarning("No SMS recipients configured for booking notification");
            return;
        }

        decimal? amount = evt.AmountTotal / 100M;
        string price = amount % 1 > 0 ? $"{amount:C2}" : $"{amount:C0}";
        string message = $"{evt.Name} booked {evt.PropertyName} {evt.CheckIn:dd/MM/yyyy} to {evt.CheckOut:dd/MM/yyyy} for {price}, " +
                         $"telephone: {evt.Telephone}, email: {evt.Email}";

        await textMessageService.SendText(message, evt.SmsRecipients);

        logger.LogInformation(
            "SMS notification sent for booking: {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);
    }
}
