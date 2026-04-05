namespace LymmHolidayLets.Contracts;

/// <summary>
/// Published when a Stripe checkout.session.completed webhook has been processed
/// and the booking has been persisted. Consumed by the EmailWorker to send confirmation
/// emails to both the company inbox and the customer.
/// </summary>
public sealed record BookingConfirmedEvent(
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
    long? AmountTotal);
