using LymmHolidayLets.Domain.Dto.Email;

namespace LymmHolidayLets.Domain.Interface
{
	public interface IEmailTemplateBuilder
	{
        Task<string> BuildHtmlBookingEmailToCompany(BookingConfirmationForCompany model);
        Task<string> BuildHtmlBookingEmailToCustomer(BookingConfirmationForCustomer model);
        Task<string> BuildHtmlContactToCompanyEmail(EmailEnquiryToCompany model);
    }
}