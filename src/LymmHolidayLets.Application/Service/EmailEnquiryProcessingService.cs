using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service;

public sealed class EmailEnquiryProcessingService(
    IEmailEnquiryCommand emailEnquiryCommand,
    IPublishEndpoint publishEndpoint,
    IRecaptchaValidationService recaptchaValidationService,
    ILogger<EmailEnquiryProcessingService> logger) : IEmailEnquiryProcessingService
{
    public async Task<EmailEnquiryResponse> ProcessEnquiryAsync(
        EmailEnquirySubmission request,
        CancellationToken cancellationToken = default)
    {
        var recaptchaValid = await recaptchaValidationService.ValidateAsync(request.ReCaptchaToken, cancellationToken);
        if (!recaptchaValid)
        {
            logger.LogWarning(
                "ReCaptcha verification failed for EmailAddress={EmailAddress} ClientIP={ClientIP}",
                request.EmailAddress,
                request.ClientIp);
            return EmailEnquiryResponse.Failure("Security verification failed. Please try again.");
        }

        var enquiryTime = DateTime.UtcNow;

        emailEnquiryCommand.Create(new EmailEnquiry
        {
            Name = request.Name,
            Company = request.Company,
            EmailAddress = request.EmailAddress,
            TelephoneNo = request.TelephoneNo,
            Subject = request.Subject,
            Message = request.Message,
            DateTimeOfEnquiry = enquiryTime
        });

        logger.LogInformation("Email enquiry saved for {EmailAddress}", request.EmailAddress);

        await publishEndpoint.Publish(new EmailEnquirySubmittedEvent(
            request.Name,
            request.Company,
            request.EmailAddress,
            request.TelephoneNo,
            request.Subject,
            request.Message,
            enquiryTime), cancellationToken);

        return EmailEnquiryResponse.Success();
    }
}
