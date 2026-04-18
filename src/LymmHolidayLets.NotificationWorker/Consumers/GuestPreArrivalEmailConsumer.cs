using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.ReadModel.Email;
using MassTransit;

namespace LymmHolidayLets.NotificationWorker.Consumers;

public sealed class GuestPreArrivalEmailConsumer(
    IGuestPreArrivalEmailDataAdapter guestPreArrivalEmailDataAdapter,
    IEmailGeneratorService emailGeneratorService,
    ILogger<GuestPreArrivalEmailConsumer> logger) : IConsumer<GuestPreArrivalEmailRequested>
{
    private const string EmailType = "PreArrivalGuest";

    public async Task Consume(ConsumeContext<GuestPreArrivalEmailRequested> context)
    {
        var evt = context.Message;

        logger.LogInformation(
            "Sending guest pre-arrival email for BookingId={BookingId} PropertyId={PropertyId} GuestEmail={GuestEmail}",
            evt.BookingId,
            evt.PropertyId,
            evt.GuestEmail);

        try
        {
            var template = guestPreArrivalEmailDataAdapter.GetTemplateByPropertyId(evt.PropertyId)
                ?? throw new InvalidOperationException(
                    $"Guest pre-arrival email template is not configured for property {evt.PropertyId}.");

            if (!template.IsEnabled)
            {
                throw new InvalidOperationException(
                    $"Guest pre-arrival email schedule is disabled for property {evt.PropertyId}.");
            }

            var model = new GuestPreArrivalEmail
            {
                PropertyName = template.PropertyName,
                BookingReference = evt.BookingReference,
                CheckIn = evt.CheckIn,
                CheckOut = evt.CheckOut,
                Name = evt.GuestName,
                Email = evt.GuestEmail,
                Telephone = evt.GuestTelephone,
                PostalCode = evt.GuestPostalCode,
                Country = evt.GuestCountry,
                Total = evt.AmountTotal,
                CheckInTimeAfter = template.CheckInTimeAfter,
                CheckOutTimeBefore = template.CheckOutTimeBefore,
                FullAddress = FormatAddress(template),
                DirectionsUrl = template.DirectionsUrl,
                ArrivalInstructions = template.ArrivalInstructions,
                SubjectTemplate = template.SubjectTemplate,
                PreviewTextTemplate = template.PreviewTextTemplate,
                HtmlBodyTemplate = template.HtmlBody
            };

            await emailGeneratorService.EmailGuestPreArrivalToCustomer(model);

            guestPreArrivalEmailDataAdapter.UpdateDispatchStatus(
                evt.BookingId,
                EmailType,
                status: "Sent");

            logger.LogInformation(
                "Guest pre-arrival email sent for BookingId={BookingId} GuestEmail={GuestEmail}",
                evt.BookingId,
                evt.GuestEmail);
        }
        catch (Exception ex)
        {
            guestPreArrivalEmailDataAdapter.UpdateDispatchStatus(
                evt.BookingId,
                EmailType,
                status: "Failed",
                failureMessage: ex.Message);

            logger.LogError(
                ex,
                "Failed sending guest pre-arrival email for BookingId={BookingId} GuestEmail={GuestEmail}",
                evt.BookingId,
                evt.GuestEmail);

            throw;
        }
    }

    private static string FormatAddress(PropertyGuestEmailTemplateData template)
    {
        var parts = new[]
        {
            template.AddressLineOne,
            template.AddressLineTwo,
            template.TownOrCity,
            template.County,
            template.Postcode,
            template.Country
        };

        return string.Join(", ", parts.Where(static part => !string.IsNullOrWhiteSpace(part)));
    }
}
