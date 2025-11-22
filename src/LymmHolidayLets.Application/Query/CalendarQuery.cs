using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Dto;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.ReadModel.Calendar;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Application.Query
{
    public sealed class CalendarQuery(IDapperCalendarDataAdapter calendarDataAdapter,
        IDapperCalendarRepository calendarRepository, ICalendarRepositoryEF calendarRepositoryEF) : ICalendarQuery
    {
        private readonly IDapperCalendarDataAdapter _calendarDataAdapter = calendarDataAdapter;
        private readonly IDapperCalendarRepository _calendarRepository = calendarRepository;
        private readonly ICalendarRepositoryEF _calendarRepositoryEF = calendarRepositoryEF;
        //public async Task<CalendarDto?> GetCalendarById(int id)
        public IQueryable<CalendarEF> GetCalendarById(int id)
        {
            return _calendarRepositoryEF.GetCalendarByIdAsync(id); ;
            //var calendar = _calendarRepository.GetById(id);
            //var calendar = await _calendarRepositoryEF.GetCalendarByIdAsync(id);

            //if (calendar == null)
            //{
            //    return null;
            //}

            //return new CalendarDto
            //{
            //    ID = calendar.ID,
            //    PropertyID = calendar.PropertyID,
            //    Date = calendar.Date,
            //    Price = calendar.Price,
            //    MinimumStay = calendar.MinimumStay,
            //    MaximumStay = calendar.MaximumStay != null ? (short?)calendar.MaximumStay : null,
            //    Available = calendar.Available,
            //    Booked = calendar.Booked,
            //    BookingID = calendar.BookingID
            //};
        }

        //public Task<CalendarEF?> GetByIdEF(int id)
        //{
        //    return _calendarRepositoryEF.GetCalendarByIdAsync(id);
        //}

        public Domain.Model.Calendar.Entity.Calendar? GetById(int id)
        {
            return _calendarRepository.GetById(id);
        }

        public IEnumerable<Domain.Model.Calendar.Entity.Calendar> GetByPropertyIDDate(byte propertyId, DateOnly startDate, DateOnly endDate) 
        {
            return _calendarRepository.GetByPropertyIDDate(propertyId, startDate, endDate);
        }

        public IEnumerable<Domain.ReadModel.Calendar.Calendar> GetPropertyAvailableBetweenDates(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available)
        {
            return _calendarDataAdapter.GetPropertyAvailableBetweenDates(propertyId, checkIn, checkOut, available);
        }
    }
}
