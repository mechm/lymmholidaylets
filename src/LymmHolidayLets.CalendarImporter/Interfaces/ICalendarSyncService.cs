namespace LymmHolidayLets.CalendarImporter.Interfaces;

public interface ICalendarSyncService
{
    Task SyncAllCalendarsAsync(List<PropertyCalendarConfig> properties, CancellationToken cancellationToken);
}
