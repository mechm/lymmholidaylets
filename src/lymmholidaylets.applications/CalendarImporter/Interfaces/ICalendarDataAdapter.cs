namespace LymmHolidayLets.CalendarImporter.Interfaces;

public interface ICalendarDataAdapter
{
    void BlockCalendarByPropertyForDate(int propertyId, DateOnly startDate, DateOnly endDate);
}
