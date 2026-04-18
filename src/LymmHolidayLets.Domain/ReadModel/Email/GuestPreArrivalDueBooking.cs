namespace LymmHolidayLets.Domain.ReadModel.Email;

public sealed class GuestPreArrivalDueBooking
{
    public required int BookingId { get; init; }
    public string? BookingReference { get; init; }
    public required byte PropertyId { get; init; }
    public required string PropertyName { get; init; }
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public byte? NoAdult { get; init; }
    public byte? NoChildren { get; init; }
    public byte? NoInfant { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? Telephone { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public long? Total { get; init; }
    public short SendDaysBeforeCheckIn { get; init; }
    public DateOnly ScheduledSendDate { get; init; }
}
