using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Dto;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Calendar;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class CalendarQuery(IDapperCalendarDataAdapter calendarDataAdapter,
        IDapperCalendarRepository calendarRepository) : ICalendarQuery
    {
        private readonly IDapperCalendarDataAdapter _calendarDataAdapter = calendarDataAdapter;
        private readonly IDapperCalendarRepository _calendarRepository = calendarRepository;

        public CalendarDto? GetCalendarById(int id)
        {
            var calendar = _calendarRepository.GetById(id);

            if (calendar == null)
            {
                return null;
            }

            return new CalendarDto
            {
                ID = calendar.ID,
                PropertyID = calendar.PropertyID,
                Date = calendar.Date,
                Price = calendar.Price,
                MinimumStay = calendar.MinimumStay,
                MaximumStay = calendar.MaximumStay != null ? (short?)calendar.MaximumStay : null,
                Available = calendar.Available,
                Booked = calendar.Booked,
                BookingID = calendar.BookingID
            };
        }

        public Domain.Model.Calendar.Entity.Calendar? GetById(int id)
        {
            return _calendarRepository.GetById(id);
        }

        public IEnumerable<Domain.Model.Calendar.Entity.Calendar> GetByPropertyIDDate(byte propertyId, DateOnly startDate, DateOnly endDate) 
        {
            return _calendarRepository.GetByPropertyIDDate(propertyId, startDate, endDate);
        }

        public IEnumerable<Calendar> GetPropertyAvailableBetweenDates(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available)
        {
            return _calendarDataAdapter.GetPropertyAvailableBetweenDates(propertyId, checkIn, checkOut, available);
        }
    }
}
