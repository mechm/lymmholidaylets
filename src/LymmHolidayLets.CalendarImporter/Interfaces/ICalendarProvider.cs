namespace LymmHolidayLets.CalendarImporter.Interfaces;

public interface ICalendarProvider
{
    string ProviderName { get; }
    Task<List<CalendarBlock>> FetchCalendarBlocksAsync(string url, CancellationToken cancellationToken);
}

public sealed class CalendarBlock
{
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
