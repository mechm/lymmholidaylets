namespace LymmHolidayLets.Domain.Dto.Email;

public sealed class GuestPreArrivalEmail
{
    public string PropertyName { get; init; } = string.Empty;
    public string? BookingReference { get; init; }
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Telephone { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public long? Total { get; init; }
    public TimeOnly CheckInTimeAfter { get; init; }
    public TimeOnly CheckOutTimeBefore { get; init; }
    public string FullAddress { get; init; } = string.Empty;
    public string? DirectionsUrl { get; init; }
    public string? ArrivalInstructions { get; init; }
    public string? SubjectTemplate { get; init; }
    public string? PreviewTextTemplate { get; init; }
    public string HtmlBodyTemplate { get; init; } = string.Empty;
}
