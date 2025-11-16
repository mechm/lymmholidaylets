using LymmHolidayLets.Domain.Model.Booking.Entity;

namespace LymmHolidayLets.Domain.Repository
{
	public interface IDapperBookingRepository : IDapperRepository<Booking>
	{
		Booking? GetById(int id);
		IEnumerable<Booking> GetAll();
        void Create(Booking booking);
		void Update(Booking booking);
		void Delete(int id);
    }
}
