using LymmHolidayLets.Api.Models.Email;

namespace LymmHolidayLets.Api.Services
{
    public interface IEmailEnquiryService
    {
        Task ProcessEnquiryAsync(EmailEnquiryRequest request, CancellationToken cancellationToken = default);
    }
}
