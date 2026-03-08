using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Application.Query
{
    public sealed class CalendarQuery(IDapperCalendarDataAdapter calendarDataAdapter,
        ICalendarRepository calendarRepository, ICalendarRepositoryEF calendarRepositoryEf) : ICalendarQuery
    {
        private readonly IDapperCalendarDataAdapter _calendarDataAdapter = calendarDataAdapter;
        private readonly ICalendarRepository _calendarRepository = calendarRepository;
        private readonly ICalendarRepositoryEF _calendarRepositoryEf = calendarRepositoryEf;

        public IQueryable<CalendarEF> GetCalendarById(int id)
        {
            return _calendarRepositoryEf.GetCalendarByIdAsync(id);
        }

        public Calendar? GetById(int id)
        {
            return _calendarRepository.GetById(id);
        }

        public IEnumerable<Calendar> GetByPropertyIDDate(byte propertyId, DateOnly startDate, DateOnly endDate) 
        {
            return _calendarRepository.GetByPropertyIDDate(propertyId, startDate, endDate);
        }

        public IEnumerable<Domain.ReadModel.Calendar.Calendar> GetPropertyAvailableBetweenDates(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available)
        {
            return _calendarDataAdapter.GetPropertyAvailableBetweenDates(propertyId, checkIn, checkOut, available);
        }
    }
}
