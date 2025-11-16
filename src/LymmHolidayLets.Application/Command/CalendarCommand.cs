using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
	public sealed class CalendarCommand : ICalendarCommand
	{
		private readonly IDapperCalendarRepository _calendarRepository;
		public CalendarCommand(IDapperCalendarRepository calendarRepository)
		{
			_calendarRepository = calendarRepository;
		}

		public void Create(Calendar calendar)
		{
			_calendarRepository.Create(
				new Domain.Model.Calendar.Entity.Calendar(calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, calendar.Booked, calendar.BookingID));
		}

		public void Update(IEnumerable<int> ids, Calendar calendar) 
		{
            _calendarRepository.Update(ids,
                    new Domain.Model.Calendar.Entity.Calendar(calendar.ID, calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, calendar.Booked, calendar.BookingID));
        }

        public void Update(Calendar calendar)
		{
			_calendarRepository.Update(
				new Domain.Model.Calendar.Entity.Calendar(calendar.ID, calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, calendar.Booked, calendar.BookingID));
        }

        public void Delete(int id)
        {
            _calendarRepository.Delete(id);
        }
    }
}
