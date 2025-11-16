using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperCalendarDataAdapter : IDapperSqlQueryBase    
    {
        bool GetPropertyAvailableForDate(byte propertyId, DateOnly date);
        IEnumerable<ReadModel.Calendar.Calendar> GetPropertyAvailableBetweenDates(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available);
    }
}