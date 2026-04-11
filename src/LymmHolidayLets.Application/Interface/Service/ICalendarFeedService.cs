using LymmHolidayLets.Application.Model.Service;

namespace LymmHolidayLets.Application.Interface.Service;

public interface ICalendarFeedService
{
    Task<CalendarFeedResult?> GetCalendarAsync(
        byte propertyId,
        Guid identifier,
        CancellationToken cancellationToken = default);
}
