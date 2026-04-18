using LymmHolidayLets.Domain.Dto.Email;

namespace LymmHolidayLets.Application.Interface.Service
{
	public interface IEmailTemplateBuilder
	{
        Task<string> BuildHtmlBookingEmailToCompany(BookingConfirmationForCompany model);
        Task<string> BuildHtmlBookingEmailToCustomer(BookingConfirmationForCustomer model);
        Task<string> BuildHtmlContactToCompanyEmail(EmailEnquiryToCompany model);
        Task<string> BuildHtmlGuestPreArrivalEmail(GuestPreArrivalEmail model);
        Task<string> BuildSubjectGuestPreArrivalEmail(GuestPreArrivalEmail model);
    }
}
