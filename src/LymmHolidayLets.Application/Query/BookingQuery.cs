using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Booking.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class BookingQuery : IBookingQuery
    {
        private readonly IDapperBookingRepository _bookingRepository;
        public BookingQuery(IDapperBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public Booking? GetById(int id)
        {
            return _bookingRepository.GetById(id);
        }

        public IEnumerable<Booking> GetAll()
        {
            return _bookingRepository.GetAll();
        }
    }
}