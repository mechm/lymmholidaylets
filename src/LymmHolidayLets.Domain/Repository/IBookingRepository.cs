using LymmHolidayLets.Domain.Model.Booking.Entity;


namespace LymmHolidayLets.Domain.Repository
{
	public interface IBookingRepository : IRepository<Booking>
	{
		Booking? GetById(int id);
		IEnumerable<Booking> GetAll();
        void Create(Booking booking);
		void Update(Booking booking);
		void Delete(int id);
    }
}
