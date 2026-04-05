using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.Dto.Email;
using MassTransit;

namespace LymmHolidayLets.EmailWorker.Consumers;

/// <summary>
/// Sends a booking confirmation email to the customer when a booking is confirmed.
/// Intentionally separate from <see cref="BookingConfirmedToCompanyConsumer"/> so that
/// a failure in one does not cause the other to retry and produce a duplicate email.
/// Each consumer binds its own queue to the BookingConfirmedEvent exchange; RabbitMQ
/// delivers an independent copy of every event to each queue.
/// </summary>
public sealed class BookingConfirmedToCustomerConsumer(
    IEmailGeneratorService emailGeneratorService,
    ILogger<BookingConfirmedToCustomerConsumer> logger) : IConsumer<BookingConfirmedEvent>
{
    public async Task Consume(ConsumeContext<BookingConfirmedEvent> context)
    {
        var evt = context.Message;

        logger.LogInformation(
            "Sending customer booking confirmation for {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);

        var model = new BookingConfirmationForCustomer(
            evt.PropertyName, evt.CheckIn, evt.CheckOut,
            evt.NoAdult, evt.NoChildren, evt.NoInfant,
            evt.Name, evt.Email ?? string.Empty, evt.Telephone ?? string.Empty,
            evt.PostalCode ?? string.Empty, evt.Country ?? string.Empty, evt.AmountTotal);

        await emailGeneratorService.EmailBookingConfirmationToCustomer(model);

        logger.LogInformation(
            "Customer booking confirmation sent for {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);
    }
}

