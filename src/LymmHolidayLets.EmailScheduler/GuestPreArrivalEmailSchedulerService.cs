using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.DataAdapter;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LymmHolidayLets.EmailScheduler;

public sealed class GuestPreArrivalEmailSchedulerService(
    IGuestPreArrivalEmailDataAdapter guestPreArrivalEmailDataAdapter,
    IPublishEndpoint publishEndpoint,
    IOptions<GuestPreArrivalEmailSchedulerOptions> options,
    ILogger<GuestPreArrivalEmailSchedulerService> logger)
{
    private const string EmailType = "PreArrivalGuest";

    public async Task ProcessDueEmailsAsync(CancellationToken cancellationToken)
    {
        var runDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var dueBookings = guestPreArrivalEmailDataAdapter.GetDueBookings(runDate);

        var publishedCount = 0;
        var skippedCount = 0;

        foreach (var booking in dueBookings)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var scheduledForUtc = ToUtcStartOfDay(booking.ScheduledSendDate);
            var reservationExpiresAtUtc = DateTime.UtcNow.AddMinutes(options.Value.ReservationTimeoutMinutes);

            var reserved = guestPreArrivalEmailDataAdapter.TryReserveDispatch(
                booking.BookingId,
                EmailType,
                scheduledForUtc,
                reservationExpiresAtUtc);

            if (!reserved)
            {
                skippedCount++;
                continue;
            }

            try
            {
                await publishEndpoint.Publish(new GuestPreArrivalEmailRequested(
                    booking.BookingId,
                    booking.BookingReference,
                    booking.PropertyId,
                    booking.PropertyName,
                    booking.CheckIn,
                    booking.CheckOut,
                    booking.NoAdult,
                    booking.NoChildren,
                    booking.NoInfant,
                    booking.Name,
                    booking.Email,
                    booking.Telephone,
                    booking.PostalCode,
                    booking.Country,
                    booking.Total,
                    booking.ScheduledSendDate), cancellationToken);

                guestPreArrivalEmailDataAdapter.UpdateDispatchStatus(
                    booking.BookingId,
                    EmailType,
                    status: "Published");

                publishedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed publishing guest pre-arrival email request for BookingId={BookingId} PropertyId={PropertyId} GuestEmail={GuestEmail}",
                    booking.BookingId,
                    booking.PropertyId,
                    booking.Email);

                guestPreArrivalEmailDataAdapter.UpdateDispatchStatus(
                    booking.BookingId,
                    EmailType,
                    status: "Failed",
                    failureMessage: ex.Message);
            }
        }

        logger.LogInformation(
            "Processed guest pre-arrival email schedule for RunDate={RunDate}: Due={DueCount}, Published={PublishedCount}, Skipped={SkippedCount}",
            runDate,
            dueBookings.Count,
            publishedCount,
            skippedCount);
    }

    private static DateTime ToUtcStartOfDay(DateOnly date) =>
        new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
}
