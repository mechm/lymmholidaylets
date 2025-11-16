using LymmHolidayLets.Domain.Model.Booking.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IBookingQuery
    {
        Booking? GetById(int id);
        IEnumerable<Booking> GetAll();
    }
}