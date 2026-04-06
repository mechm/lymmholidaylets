namespace LymmHolidayLets.CalendarImporter.Interfaces;

public interface ICalendarProviderFactory
{
    ICalendarProvider? GetProvider(string providerName);
}
