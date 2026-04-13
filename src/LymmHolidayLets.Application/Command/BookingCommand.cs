using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Model.Booking.ValueObject;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class BookingCommand(IBookingRepository bookingRepository) : IBookingCommand
    {
		public void Create(LymmHolidayLets.Application.Model.Command.Booking booking)
		{
			bookingRepository.Create(
                Domain.Model.Booking.Entity.Booking.CreateNew(
                    booking.EventID,
                    booking.SessionID,
                    booking.PropertyID,
                    new StayPeriod(booking.CheckIn, booking.CheckOut),
                    new ContactInfo(booking.Name, booking.Email, booking.Telephone),
                    booking.NoAdult,
                    booking.NoChildren,
                    booking.NoInfant,
                    booking.PostalCode,
                    booking.Country,
                    booking.Total));
		}

        public void Update(LymmHolidayLets.Application.Model.Command.Booking booking)
        {
            bookingRepository.Update(
                new Domain.Model.Booking.Entity.Booking(booking.ID, booking.EventID, booking.SessionID, booking.PropertyID, booking.CheckIn, booking.CheckOut, booking.NoAdult, booking.NoChildren,
                booking.NoInfant, booking.Name, booking.Email, booking.Telephone, booking.PostalCode, booking.Country, booking.Total));
        }

		public void Delete(int id) 
		{
			bookingRepository.Delete(id);
        }
    }
}
