using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Contracts;
using MassTransit;

namespace LymmHolidayLets.Api.Services
{
    public sealed class EmailEnquiryService(
        IEmailEnquiryCommand emailEnquiryCommand,
        IPublishEndpoint publishEndpoint,
        ILogger<EmailEnquiryService> logger) : IEmailEnquiryService
    {
        public async Task ProcessEnquiryAsync(EmailEnquiryRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Save to database
            var emailEnquiry = new EmailEnquiry
            {
                Name = request.Name,
                Company = request.Company,
                EmailAddress = request.EmailAddress,
                TelephoneNo = request.TelephoneNo,
                Subject = request.Subject,
                Message = request.Message,
                DateTimeOfEnquiry = DateTime.UtcNow
            };

            emailEnquiryCommand.Create(emailEnquiry);

            logger.LogInformation("Email enquiry saved for {EmailAddress}", request.EmailAddress);

            // 2. Publish event — the EmailWorker consumes this and sends the email
            await publishEndpoint.Publish(new EmailEnquirySubmittedEvent(
                request.Name,
                request.Company,
                request.EmailAddress,
                request.TelephoneNo,
                request.Subject,
                request.Message,
                DateTime.UtcNow), cancellationToken);
        }
    }
}