using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IBookingCommand
	{
		void Create(Booking booking);
        void Update(Booking booking);
        void Delete(int id);
    }
}