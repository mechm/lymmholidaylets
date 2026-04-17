namespace LymmHolidayLets.Contracts;

/// <summary>
/// Published when a booking has been confirmed and persisted, requesting that
/// all notifications (email + SMS) be sent to the customer and company.
/// Consumed by NotificationWorker to fan out to email and SMS consumers.
/// </summary>
public sealed record BookingNotificationRequested(
    string PropertyName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    byte? NoAdult,
    byte? NoChildren,
    byte? NoInfant,
    string Name,
    string? Email,
    string? Telephone,
    string? PostalCode,
    string? Country,
    long? AmountTotal,
    byte PropertyId,
    string[] SmsRecipients,
    string? BookingReference);
