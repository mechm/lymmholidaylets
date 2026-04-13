using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Application.Interface.Service;

namespace LymmHolidayLets.Infrastructure.Emailer
{
    public sealed class EmailTemplateBuilder : IEmailTemplateBuilder
    {
        public Task<string> BuildHtmlBookingEmailToCompany(BookingConfirmationForCompany model)
        {
            var tokens = BuildBookingTokens(
                model.PropertyName,
                model.CheckIn,
                model.CheckOut,
                model.NoAdult,
                model.NoChildren,
                model.NoInfant,
                model.Name,
                model.Email,
                model.Telephone,
                model.PostalCode,
                model.Country,
                model.Total);

            return Task.FromResult(ReplaceTokens(LoadTemplate("BookingConfirmationCompany.html"), tokens));
        }

        public Task<string> BuildHtmlBookingEmailToCustomer(BookingConfirmationForCustomer model)
        {
            var tokens = BuildBookingTokens(
                model.PropertyName,
                model.CheckIn,
                model.CheckOut,
                model.NoAdult,
                model.NoChildren,
                model.NoInfant,
                model.Name,
                model.Email,
                model.Telephone,
                model.PostalCode,
                model.Country,
                model.Total);

            return Task.FromResult(ReplaceTokens(LoadTemplate("BookingConfirmationCustomer.html"), tokens));
        }

        public Task<string> BuildHtmlContactToCompanyEmail(EmailEnquiryToCompany model)
        {
            var safeMessage = HtmlEncoder.Default.Encode(model.Message).Replace("\n", "<br/>");
            var safeName = HtmlEncoder.Default.Encode(model.Name);
            var safeCompany = model.Company != null ? HtmlEncoder.Default.Encode(model.Company) : "N/A";
            var safeSubject = model.Subject != null ? HtmlEncoder.Default.Encode(model.Subject) : "Website Enquiry";

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

        private static Dictionary<string, string> BuildBookingTokens(
            string propertyName,
            DateOnly checkIn,
            DateOnly checkOut,
            short? noAdult,
            short? noChildren,
            short? noInfant,
            string name,
            string email,
            string telephone,
            string postalCode,
            string country,
            long? total)
        {
            return new Dictionary<string, string>
            {
                ["PropertyName"] = HtmlEncoder.Default.Encode(propertyName),
                ["CheckInDate"] = HtmlEncoder.Default.Encode(checkIn.ToString("dddd dd MMMM yyyy", CultureInfo.InvariantCulture)),
                ["CheckOutDate"] = HtmlEncoder.Default.Encode(checkOut.ToString("dddd dd MMMM yyyy", CultureInfo.InvariantCulture)),
                ["Nights"] = HtmlEncoder.Default.Encode(FormatNights(checkIn, checkOut)),
                ["GuestSummary"] = HtmlEncoder.Default.Encode(FormatGuestSummary(noAdult, noChildren, noInfant)),
                ["GuestName"] = HtmlEncoder.Default.Encode(name),
                ["GuestEmail"] = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(email) ? "Not provided" : email),
                ["GuestTelephone"] = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(telephone) ? "Not provided" : telephone),
                ["GuestPostalCode"] = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(postalCode) ? "Not provided" : postalCode),
                ["GuestCountry"] = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(country) ? "Not provided" : country),
                ["TotalAmount"] = HtmlEncoder.Default.Encode(FormatTotal(total))
            };
        }

        private static string ReplaceTokens(string template, IReadOnlyDictionary<string, string> tokens)
        {
            var html = template;

            foreach (var token in tokens)
            {
                html = html.Replace("{{" + token.Key + "}}", token.Value, StringComparison.Ordinal);
            }

            return html;
        }

        private static string LoadTemplate(string fileName)
        {
            var assembly = typeof(EmailTemplateBuilder).Assembly;
            var resourceName = $"{typeof(EmailTemplateBuilder).Namespace}.Templates.{fileName}";
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Email template resource '{resourceName}' was not found.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static string FormatNights(DateOnly checkIn, DateOnly checkOut)
        {
            var nights = checkOut.DayNumber - checkIn.DayNumber;
            return nights == 1 ? "1 night" : $"{nights} nights";
        }

        private static string FormatGuestSummary(short? noAdult, short? noChildren, short? noInfant)
        {
            var parts = new List<string>();

            AddGuestPart(parts, noAdult, "adult");
            AddGuestPart(parts, noChildren, "child");
            AddGuestPart(parts, noInfant, "infant");

            return parts.Count == 0 ? "Not provided" : string.Join(", ", parts);
        }

        private static void AddGuestPart(List<string> parts, short? count, string label)
        {
            if (!count.HasValue || count.Value <= 0)
            {
                return;
            }

            parts.Add(count.Value == 1 ? $"1 {label}" : $"{count.Value} {label}s");
        }

        private static string FormatTotal(long? total)
        {
            if (!total.HasValue)
            {
                return "Not provided";
            }

            var amount = total.Value / 100m;
            return amount.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
        }
    }
}
