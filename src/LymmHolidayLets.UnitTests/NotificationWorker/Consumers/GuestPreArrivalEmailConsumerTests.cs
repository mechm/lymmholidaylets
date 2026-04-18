using FluentAssertions;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.ReadModel.Email;
using LymmHolidayLets.NotificationWorker.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.NotificationWorker.Consumers;

public class GuestPreArrivalEmailConsumerTests
{
    private readonly Mock<IGuestPreArrivalEmailDataAdapter> _guestPreArrivalEmailDataAdapter = new();
    private readonly Mock<IEmailGeneratorService> _emailGeneratorService = new();
    private readonly Mock<ILogger<GuestPreArrivalEmailConsumer>> _logger = new();

    private GuestPreArrivalEmailConsumer CreateSut() => new(
        _guestPreArrivalEmailDataAdapter.Object,
        _emailGeneratorService.Object,
        _logger.Object);

    [Fact]
    public async Task Consume_WhenTemplateExists_SendsEmailAndMarksDispatchAsSent()
    {
        var message = CreateMessage();
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.GetTemplateByPropertyId(message.PropertyId))
            .Returns(CreateTemplate());

        await CreateSut().Consume(CreateContext(message));

        _emailGeneratorService.Verify(
            x => x.EmailGuestPreArrivalToCustomer(It.Is<GuestPreArrivalEmail>(email =>
                email.PropertyName == "Lymm Village Apartment" &&
                email.Name == "Jane Smith" &&
                email.Email == "jane@example.com" &&
                email.SubjectTemplate == "Arrival details for {propertyname}" &&
                email.PreviewTextTemplate == "Arrive from {arrivaltime} on {arrivaldate}" &&
                email.HtmlBodyTemplate.Contains("{guestname}") &&
                email.FullAddress == "5 Church Road, Lymm, Cheshire, WA13 0QG, England")),
            Times.Once);

        _guestPreArrivalEmailDataAdapter.Verify(
            x => x.UpdateDispatchStatus(message.BookingId, "PreArrivalGuest", "Sent", null),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenEmailSendFails_MarksDispatchAsFailedAndRethrows()
    {
        var message = CreateMessage();
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.GetTemplateByPropertyId(message.PropertyId))
            .Returns(CreateTemplate());
        _emailGeneratorService
            .Setup(x => x.EmailGuestPreArrivalToCustomer(It.IsAny<GuestPreArrivalEmail>()))
            .ThrowsAsync(new InvalidOperationException("smtp offline"));

        var act = async () => await CreateSut().Consume(CreateContext(message));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("smtp offline");

        _guestPreArrivalEmailDataAdapter.Verify(
            x => x.UpdateDispatchStatus(message.BookingId, "PreArrivalGuest", "Failed", "smtp offline"),
            Times.Once);
    }

    private static ConsumeContext<GuestPreArrivalEmailRequested> CreateContext(GuestPreArrivalEmailRequested message)
    {
        var context = new Mock<ConsumeContext<GuestPreArrivalEmailRequested>>();
        context.SetupGet(x => x.Message).Returns(message);
        return context.Object;
    }

    private static GuestPreArrivalEmailRequested CreateMessage() =>
        new(
            BookingId: 42,
            BookingReference: "cs_test_PREARRIVAL001",
            PropertyId: 1,
            PropertyName: "Lymm Village Apartment",
            CheckIn: new DateOnly(2026, 10, 29),
            CheckOut: new DateOnly(2026, 11, 2),
            NoAdult: 2,
            NoChildren: 1,
            NoInfant: 0,
            GuestName: "Jane Smith",
            GuestEmail: "jane@example.com",
            GuestTelephone: "07700900123",
            GuestPostalCode: "WA13 0QG",
            GuestCountry: "England",
            AmountTotal: 52500,
            ScheduledSendDate: new DateOnly(2026, 10, 24));

    private static PropertyGuestEmailTemplateData CreateTemplate() => new()
    {
        PropertyId = 1,
        PropertyName = "Lymm Village Apartment",
        IsEnabled = true,
        SendDaysBeforeCheckIn = 5,
        SubjectTemplate = "Arrival details for {propertyname}",
        PreviewTextTemplate = "Arrive from {arrivaltime} on {arrivaldate}",
        HtmlBody = "<p>Hello {guestname}</p><p>You are welcome to arrive from {arrivaltime} on {arrivaldate}.</p>",
        CheckInTimeAfter = new TimeOnly(15, 0),
        CheckOutTimeBefore = new TimeOnly(10, 0),
        AddressLineOne = "5 Church Road",
        AddressLineTwo = null,
        TownOrCity = "Lymm",
        County = "Cheshire",
        Postcode = "WA13 0QG",
        Country = "England",
        DirectionsUrl = "https://maps.example.com/route",
        ArrivalInstructions = "Collect the key from the lockbox."
    };
}
