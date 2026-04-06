using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LymmHolidayLets.NotificationWorker.Consumers;

public sealed class EmailEnquiryConsumer(
    IEmailTemplateBuilder emailTemplateBuilder,
    IEmailService emailService,
    IOptions<EmailOptions> emailOptions,
    ILogger<EmailEnquiryConsumer> logger) : IConsumer<EmailEnquirySubmittedEvent>
{
    public async Task Consume(ConsumeContext<EmailEnquirySubmittedEvent> context)
    {
        var evt = context.Message;
        var options = emailOptions.Value;

        logger.LogInformation(
            "Processing enquiry email for {EmailAddress}", evt.EmailAddress);

        var emailDto = new EmailEnquiryToCompany
        {
            Name = evt.Name,
            Company = evt.Company,
            EmailAddress = evt.EmailAddress,
            TelephoneNo = evt.TelephoneNo,
            Subject = evt.Subject,
            Message = evt.Message,
            Created = evt.SubmittedAt
        };

        var html = await emailTemplateBuilder.BuildHtmlContactToCompanyEmail(emailDto);

        var ccEmails = options.CcEmails
            .ToDictionary(kvp => kvp.Key, kvp => (string?)kvp.Value);

        await emailService.SendAsync(new EmailMessage
        {
            ToName = options.CompanyName,
            ToEmailAddress = options.CompanyEmail,
            CcEmailAddress = ccEmails,
            Subject = evt.Subject ?? "New Website Enquiry"
        }, html);

        logger.LogInformation(
            "Enquiry email sent successfully for {EmailAddress}", evt.EmailAddress);
    }
}
