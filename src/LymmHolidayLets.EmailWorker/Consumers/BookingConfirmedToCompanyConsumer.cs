using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.Dto.Email;
using MassTransit;

namespace LymmHolidayLets.EmailWorker.Consumers;

/// <summary>
/// Sends a booking confirmation email to the company when a booking is confirmed.
/// Intentionally separate from <see cref="BookingConfirmedToCustomerConsumer"/> so that
/// a failure in one does not cause the other to retry and produce a duplicate email.
/// Each consumer binds its own queue to the BookingConfirmedEvent exchange; RabbitMQ
/// delivers an independent copy of every event to each queue.
/// </summary>
public sealed class BookingConfirmedToCompanyConsumer(
    IEmailGeneratorService emailGeneratorService,
    ILogger<BookingConfirmedToCompanyConsumer> logger) : IConsumer<BookingConfirmedEvent>
{
    public async Task Consume(ConsumeContext<BookingConfirmedEvent> context)
    {
        var evt = context.Message;

        logger.LogInformation(
            "Sending company booking confirmation for {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);

        var model = new BookingConfirmationForCompany(
            evt.PropertyName, evt.CheckIn, evt.CheckOut,
            evt.NoAdult, evt.NoChildren, evt.NoInfant,
            evt.Name, evt.Email ?? string.Empty, evt.Telephone ?? string.Empty,
            evt.PostalCode ?? string.Empty, evt.Country ?? string.Empty, evt.AmountTotal);

        await emailGeneratorService.EmailBookingConfirmationToCompany(model);

        logger.LogInformation(
            "Company booking confirmation sent for {PropertyName}, Guest={GuestName}",
            evt.PropertyName, evt.Name);
    }
}

