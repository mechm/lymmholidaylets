using LymmHolidayLets.Application.Model.Service;

namespace LymmHolidayLets.Application.Interface.Service;

public interface IEmailEnquiryProcessingService
{
    Task<EmailEnquiryResponse> ProcessEnquiryAsync(
        EmailEnquirySubmission request,
        CancellationToken cancellationToken = default);
}
