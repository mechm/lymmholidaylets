using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Api.Services
{
    public sealed class EmailEnquiryService(
        IEmailEnquiryCommand emailEnquiryCommand,
        IEmailService emailService,
        IEmailTemplateBuilder emailTemplateBuilder,
        IConfiguration configuration,
        ILogger<EmailEnquiryService> logger) : IEmailEnquiryService
    {
        private readonly IDictionary<string, string?>? _ccEmails = configuration.GetSection("Keys:CCEmail").Get<Dictionary<string, string?>>();

        public Task<bool> ProcessEnquiryAsync(EmailEnquiryRequest request, CancellationToken cancellationToken = default)
        {
            try
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

                // 2. Send email in background (async but properly handled)
                _ = Task.Run(async () => await SendEnquiryEmailAsync(request, cancellationToken), cancellationToken);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process email enquiry for {EmailAddress}", request.EmailAddress);
                return Task.FromResult(false);
            }
        }

        private async Task SendEnquiryEmailAsync(EmailEnquiryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var companyEmail = configuration["Email:CompanyEmail"] ?? "matthew@lymmholidaylets.com";
                
                var emailDto = new EmailEnquiryToCompany
                {
                    Name = request.Name,
                    Company = request.Company,
                    EmailAddress = request.EmailAddress,
                    TelephoneNo = request.TelephoneNo,
                    Subject = request.Subject,
                    Message = request.Message,
                    Created = DateTime.UtcNow
                };

                var htmlBody = await emailTemplateBuilder.BuildHtmlContactToCompanyEmail(emailDto);
                
                var message = new EmailMessage
                {
                    ToName = configuration["Email:CompanyName"] ?? "Lymm Holiday Lets",
                    ToEmailAddress = companyEmail,
                    CcEmailAddress = _ccEmails,
                    Subject = request.Subject ?? "New Website Enquiry"
                };

                await emailService.SendAsync(message, htmlBody);
                
                logger.LogInformation("Email sent successfully for enquiry from {EmailAddress}", request.EmailAddress);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send enquiry email for {EmailAddress}", request.EmailAddress);
                // Could add retry logic here or queue for later retry
            }
        }
    }
}
