namespace LymmHolidayLets.Domain.ReadModel.Email;

public sealed class PropertyGuestEmailTemplateData
{
    public required byte PropertyId { get; init; }
    public required string PropertyName { get; init; }
    public bool IsEnabled { get; init; }
    public short SendDaysBeforeCheckIn { get; init; }
    public string? SubjectTemplate { get; init; }
    public string? PreviewTextTemplate { get; init; }
    public required string HtmlBody { get; init; }
    public TimeOnly CheckInTimeAfter { get; init; }
    public TimeOnly CheckOutTimeBefore { get; init; }
    public string? AddressLineOne { get; init; }
    public string? AddressLineTwo { get; init; }
    public string? TownOrCity { get; init; }
    public string? County { get; init; }
    public string? Postcode { get; init; }
    public string? Country { get; init; }
    public string? DirectionsUrl { get; init; }
    public string? ArrivalInstructions { get; init; }
}
