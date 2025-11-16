using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class BookingCommand : IBookingCommand
	{
		private readonly IDapperBookingRepository _bookingRepository;
		public BookingCommand(IDapperBookingRepository bookingRepository)
		{
			_bookingRepository = bookingRepository;
		}

		public void Create(Booking booking)
		{
			_bookingRepository.Create(
				new Domain.Model.Booking.Entity.Booking(booking.EventID, booking.SessionID, booking.PropertyID, booking.CheckIn, booking.CheckOut, booking.NoAdult, booking.NoChildren,
				booking.NoInfant, booking.Name,	booking.Email, booking.Telephone, booking.PostalCode, booking.Country, booking.Total));
		}

        public void Update(Booking booking)
        {
            _bookingRepository.Update(
                new Domain.Model.Booking.Entity.Booking(booking.ID, booking.EventID, booking.SessionID, booking.PropertyID, booking.CheckIn, booking.CheckOut, booking.NoAdult, booking.NoChildren,
                booking.NoInfant, booking.Name, booking.Email, booking.Telephone, booking.PostalCode, booking.Country, booking.Total));
        }

		public void Delete(int id) 
		{
			_bookingRepository.Delete(id);
        }
    }
}