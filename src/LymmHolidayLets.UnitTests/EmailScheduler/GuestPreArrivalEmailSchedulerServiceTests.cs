using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Email;
using LymmHolidayLets.EmailScheduler;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.EmailScheduler;

public class GuestPreArrivalEmailSchedulerServiceTests
{
    private readonly Mock<IGuestPreArrivalEmailDataAdapter> _guestPreArrivalEmailDataAdapter = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();
    private readonly Mock<ILogger<GuestPreArrivalEmailSchedulerService>> _logger = new();

    private GuestPreArrivalEmailSchedulerService CreateSut() => new(
        _guestPreArrivalEmailDataAdapter.Object,
        _publishEndpoint.Object,
        Options.Create(new GuestPreArrivalEmailSchedulerOptions
        {
            IntervalMinutes = 30,
            ReservationTimeoutMinutes = 15
        }),
        _logger.Object);

    [Fact]
    public async Task ProcessDueEmailsAsync_WhenDueBookingExists_PublishesEventAndMarksPublished()
    {
        var booking = CreateDueBooking();
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.GetDueBookings(It.IsAny<DateOnly?>()))
            .Returns([booking]);
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.TryReserveDispatch(
                booking.BookingId,
                "PreArrivalGuest",
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(true);

        await CreateSut().ProcessDueEmailsAsync(CancellationToken.None);

        _publishEndpoint.Verify(
            x => x.Publish(
                It.Is<GuestPreArrivalEmailRequested>(evt =>
                    evt.BookingId == booking.BookingId &&
                    evt.PropertyId == booking.PropertyId &&
                    evt.PropertyName == booking.PropertyName &&
                    evt.GuestName == booking.Name &&
                    evt.GuestEmail == booking.Email &&
                    evt.ScheduledSendDate == booking.ScheduledSendDate),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _guestPreArrivalEmailDataAdapter.Verify(
            x => x.UpdateDispatchStatus(booking.BookingId, "PreArrivalGuest", "Published", null),
            Times.Once);
    }

    [Fact]
    public async Task ProcessDueEmailsAsync_WhenReservationCannotBeTaken_DoesNotPublish()
    {
        var booking = CreateDueBooking();
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.GetDueBookings(It.IsAny<DateOnly?>()))
            .Returns([booking]);
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.TryReserveDispatch(
                booking.BookingId,
                "PreArrivalGuest",
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(false);

        await CreateSut().ProcessDueEmailsAsync(CancellationToken.None);

        _publishEndpoint.Verify(
            x => x.Publish(It.IsAny<GuestPreArrivalEmailRequested>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _guestPreArrivalEmailDataAdapter.Verify(
            x => x.UpdateDispatchStatus(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessDueEmailsAsync_WhenPublishFails_MarksDispatchAsFailed()
    {
        var booking = CreateDueBooking();
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.GetDueBookings(It.IsAny<DateOnly?>()))
            .Returns([booking]);
        _guestPreArrivalEmailDataAdapter
            .Setup(x => x.TryReserveDispatch(
                booking.BookingId,
                "PreArrivalGuest",
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(true);
        _publishEndpoint
            .Setup(x => x.Publish(It.IsAny<GuestPreArrivalEmailRequested>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("broker unavailable"));

        await CreateSut().ProcessDueEmailsAsync(CancellationToken.None);

        _guestPreArrivalEmailDataAdapter.Verify(
            x => x.UpdateDispatchStatus(booking.BookingId, "PreArrivalGuest", "Failed", "broker unavailable"),
            Times.Once);
    }

    private static GuestPreArrivalDueBooking CreateDueBooking() => new()
    {
        BookingId = 42,
        BookingReference = "cs_test_PREARRIVAL001",
        PropertyId = 1,
        PropertyName = "Lymm Village Apartment",
        CheckIn = new DateOnly(2026, 10, 29),
        CheckOut = new DateOnly(2026, 11, 2),
        NoAdult = 2,
        NoChildren = 1,
        NoInfant = 0,
        Name = "Jane Smith",
        Email = "jane@example.com",
        Telephone = "07700900123",
        PostalCode = "WA13 0QG",
        Country = "United Kingdom",
        Total = 52500,
        SendDaysBeforeCheckIn = 5,
        ScheduledSendDate = new DateOnly(2026, 10, 24)
    };
}
