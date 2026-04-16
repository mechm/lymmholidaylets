using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Contracts;
using MassTransit;

namespace LymmHolidayLets.NotificationWorker.Consumers;

/// <summary>
/// Sends a booking confirmation email to the customer when a booking notification is requested.
/// Intentionally separate from <see cref="BookingConfirmedToCompanyConsumer"/> so that
/// a failure in one does not cause the other to retry and produce a duplicate email.
/// Each consumer binds its own queue to the BookingNotificationRequested exchange; RabbitMQ
/// delivers an independent copy of every event to each queue.
/// </summary>
public sealed class BookingConfirmedToCustomerConsumer(
    ICustomerBookingConfirmationBuilder customerBookingConfirmationBuilder,
    IEmailGeneratorService emailGeneratorService,
    ILogger<BookingConfirmedToCustomerConsumer> logger) : IConsumer<BookingNotificationRequested>
{
    public async Task Consume(ConsumeContext<BookingNotificationRequested> context)
    {
        var evt = context.Message;

        logger.LogInformation(
            "Sending customer booking confirmation for {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);

        var model = await customerBookingConfirmationBuilder.BuildAsync(evt);

        await emailGeneratorService.EmailBookingConfirmationToCustomer(model);

        logger.LogInformation(
            "Customer booking confirmation sent for {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);
    }
}

