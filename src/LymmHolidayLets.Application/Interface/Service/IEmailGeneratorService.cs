using LymmHolidayLets.Domain.Dto.Email;

namespace LymmHolidayLets.Application.Interface.Service
{
    public interface IEmailGeneratorService
    {
        Task EmailBookingConfirmationToCompany(BookingConfirmationForCompany bookingConfirmationForCompany);
        Task EmailBookingConfirmationToCustomer(BookingConfirmationForCustomer bookingConfirmationForCustomer);
    }
}
