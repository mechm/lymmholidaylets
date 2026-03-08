using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
	public sealed class CalendarCommand(ICalendarRepository calendarRepository) : ICalendarCommand
	{
		public void Create(Calendar calendar)
		{
			calendarRepository.Create(
				new Domain.Model.Calendar.Entity.Calendar(calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, calendar.Booked, calendar.BookingID));
		}

		public void Update(IEnumerable<int> ids, Calendar calendar) 
		{
            calendarRepository.Update(ids,
                    new Domain.Model.Calendar.Entity.Calendar(calendar.ID, calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, calendar.Booked, calendar.BookingID));
        }

        public void Update(Calendar calendar)
		{
			calendarRepository.Update(
				new Domain.Model.Calendar.Entity.Calendar(calendar.ID, calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, calendar.Booked, calendar.BookingID));
        }

        public void Delete(int id)
        {
            calendarRepository.Delete(id);
        }
    }
}
