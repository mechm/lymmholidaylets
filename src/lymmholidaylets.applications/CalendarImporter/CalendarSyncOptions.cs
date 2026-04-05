namespace LymmHolidayLets.CalendarImporter;

public sealed class CalendarSyncOptions
{
    public int IntervalMinutes { get; set; } = 30;
    public List<PropertyCalendarConfig> Properties { get; set; } = new();
}

public sealed class PropertyCalendarConfig
{
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public List<CalendarFeedConfig> Calendars { get; set; } = new();
}

public sealed class CalendarFeedConfig
{
    public string Provider { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
