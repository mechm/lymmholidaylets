using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.Dto.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LymmHolidayLets.Application.Service
{
    /// <summary>
    /// Generates and dispatches booking confirmation emails to both the company inbox
    /// and the customer, using HTML templates built by <see cref="IEmailTemplateBuilder"/>.
    /// </summary>
    public sealed class EmailGeneratorService(
        IOptions<EmailOptions> emailOptions,
        IEmailTemplateBuilder emailTemplateBuilder,
        IEmailService emailService,
        ILogger<EmailGeneratorService> logger) : IEmailGeneratorService
    {
        /// <summary>
        /// Sends a booking confirmation email to the company inbox (with optional CC recipients).
        /// The subject includes the property name and dates so each booking is identifiable
        /// at a glance in the inbox without opening the email.
        /// </summary>
        public async Task EmailBookingConfirmationToCompany(BookingConfirmationForCompany model)
        {
            var options = emailOptions.Value;

            var html = await emailTemplateBuilder.BuildHtmlBookingEmailToCompany(model);

            if (string.IsNullOrWhiteSpace(html))
            {
                logger.LogError(
                    "Booking confirmation HTML template was empty for PropertyName={PropertyName}, Guest={GuestName}. Email not sent.",
                    model.PropertyName, model.Name);
                return;
            }

            // Include property and dates in the subject so each booking is identifiable
            // in the inbox without opening the email.
            var subject = $"New Booking — {model.PropertyName} " +
                          $"({model.CheckIn:dd MMM yyyy} – {model.CheckOut:dd MMM yyyy})";

            // Map CcEmails dictionary to the nullable string-value format expected by EmailMessage.
            var ccEmails = options.CcEmails
                .ToDictionary(kvp => kvp.Key, string? (kvp) => kvp.Value);

            await emailService.SendAsync(
                new EmailMessage
                {
                    ToName         = options.CompanyName,
                    ToEmailAddress = options.CompanyEmail,
                    CcEmailAddress = ccEmails,
                    Subject        = subject
                }, html);

            logger.LogInformation(
                "Booking confirmation sent to company for PropertyName={PropertyName}, CheckIn={CheckIn}, CheckOut={CheckOut}, Guest={GuestName}",
                model.PropertyName, model.CheckIn, model.CheckOut, model.Name);
        }

        /// <summary>
        /// Sends a booking confirmation email to the customer using their name and email address
        /// supplied in the booking payload.
        /// </summary>
        public async Task EmailBookingConfirmationToCustomer(BookingConfirmationForCustomer model)
        {
            var html = await emailTemplateBuilder.BuildHtmlBookingEmailToCustomer(model);

            if (string.IsNullOrWhiteSpace(html))
            {
                logger.LogError(
                    "Booking confirmation HTML template was empty for Guest={GuestName}. Email not sent.",
                    model.Name);
                return;
            }

            await emailService.SendAsync(
                new EmailMessage
                {
                    ToName         = model.Name,
                    ToEmailAddress = model.Email,
                    Subject        = $"Reservation confirmed for {model.PropertyName}"
                }, html);

            logger.LogInformation(
                "Booking confirmation sent to customer Email={CustomerEmail}, PropertyName={PropertyName}",
                model.Email, model.PropertyName);
        }
    }
}
