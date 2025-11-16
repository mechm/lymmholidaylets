using LymmHolidayLets.Application.Model.Dto;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface ICalendarQuery
    {
        CalendarDto? GetCalendarById(int id);
        Domain.Model.Calendar.Entity.Calendar? GetById(int id);
        IEnumerable<Domain.Model.Calendar.Entity.Calendar> GetByPropertyIDDate(byte propertyId, DateOnly startDate, DateOnly endDate);
        IEnumerable<Domain.ReadModel.Calendar.Calendar> GetPropertyAvailableBetweenDates(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available);
    }
}
