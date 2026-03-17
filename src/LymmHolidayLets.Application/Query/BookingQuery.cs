using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Booking.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class BookingQuery(IBookingRepository bookingRepository) : IBookingQuery
    {
        public Booking? GetById(int id)
        {
            return bookingRepository.GetById(id);
        }

        public IEnumerable<Booking> GetAll()
        {
            return bookingRepository.GetAll();
        }
    }
}
