using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Infrastructure.Emailer
{
    public sealed class EmailTemplateBuilder : IEmailTemplateBuilder
    {
        public Task<string> BuildHtmlBookingEmailToCompany(BookingConfirmationForCompany model)
        {
            // Placeholder for booking email content
            return Task.FromResult($"<h3>Booking Confirmation</h3><p>Booking for {model.PropertyName} by {model.Name}</p>");
        }

        public Task<string> BuildHtmlBookingEmailToCustomer(BookingConfirmationForCustomer model)
        {
            // Placeholder for customer booking email
            return Task.FromResult($"<h3>Booking Confirmation</h3><p>Dear {model.Name}, your booking for {model.PropertyName} is confirmed.</p>");
        }

        public Task<string> BuildHtmlContactToCompanyEmail(EmailEnquiryToCompany model)
        {
            var safeMessage = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(model.Message).Replace("\n", "<br/>");
            var safeName = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(model.Name);
            var safeCompany = model.Company != null ? System.Text.Encodings.Web.HtmlEncoder.Default.Encode(model.Company) : "N/A";
            var safeSubject = model.Subject != null ? System.Text.Encodings.Web.HtmlEncoder.Default.Encode(model.Subject) : "Website Enquiry";

            return Task.FromResult($"""
                                    <h3>New Enquiry from Website</h3>
                                    <p><strong>Name:</strong> {safeName}</p>
                                    <p><strong>Company:</strong> {safeCompany}</p>
                                    <p><strong>Email:</strong> {model.EmailAddress}</p>
                                    <p><strong>Phone:</strong> {model.TelephoneNo ?? "N/A"}</p>
                                    <p><strong>Subject:</strong> {safeSubject}</p>
                                    <p><strong>Message:</strong></p>
                                    <p>{safeMessage}</p>
                                    <p><strong>Sent:</strong> {model.Created:dd/MM/yyyy HH:mm:ss}</p>
                                    """);
        }
    }
}
