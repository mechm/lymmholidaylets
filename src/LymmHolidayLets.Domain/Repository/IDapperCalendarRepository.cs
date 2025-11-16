using LymmHolidayLets.Domain.Model.Calendar.Entity;

namespace LymmHolidayLets.Domain.Repository
{
	public interface IDapperCalendarRepository : IDapperRepository<Calendar>
	{
		Calendar? GetById(int id);
        IEnumerable<Calendar> GetByPropertyIDDate(byte propertyId, DateOnly startDate, DateOnly endDate);
        void Create(Calendar calendar);
        void Update(Calendar calendar);
        void Update(IEnumerable<int> ids, Calendar calendar);
        void Delete(int id);
    }
}
