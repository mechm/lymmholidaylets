using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
	public interface ICalendarCommand
	{
		void Create(Calendar calendar);
        void Update(IEnumerable<int> ids, Calendar calendar)

        ;void Update(Calendar calendar);
		void Delete(int id);
    }
}