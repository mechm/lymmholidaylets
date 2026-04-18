namespace LymmHolidayLets.Contracts;

/// <summary>
/// Published when a booking is due for its pre-arrival guest email.
/// Consumed by NotificationWorker to build the branded email shell,
/// replace whitelisted booking/property placeholders, and send it to the guest.
/// </summary>
public sealed record GuestPreArrivalEmailRequested(
    int BookingId,
    string? BookingReference,
    byte PropertyId,
    string PropertyName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    byte? NoAdult,
    byte? NoChildren,
    byte? NoInfant,
    string GuestName,
    string GuestEmail,
    string? GuestTelephone,
    string? GuestPostalCode,
    string? GuestCountry,
    long? AmountTotal,
    DateOnly ScheduledSendDate);
