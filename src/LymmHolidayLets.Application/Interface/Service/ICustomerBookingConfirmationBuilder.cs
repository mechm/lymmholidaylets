using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.Dto.Email;

namespace LymmHolidayLets.Application.Interface.Service
{
    public interface ICustomerBookingConfirmationBuilder
    {
        Task<BookingConfirmationForCustomer> BuildAsync(BookingNotificationRequested notificationRequested);
    }
}
