using System.Globalization;
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
            var tokens = BuildCustomerBookingTokens(model);

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

        private static Dictionary<string, string> BuildCustomerBookingTokens(BookingConfirmationForCustomer model)
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

            tokens["CheckInShortDate"] = HtmlEncoder.Default.Encode(model.CheckIn.ToString("ddd, d MMM yyyy", CultureInfo.InvariantCulture));
            tokens["CheckOutShortDate"] = HtmlEncoder.Default.Encode(model.CheckOut.ToString("ddd, d MMM yyyy", CultureInfo.InvariantCulture));
            tokens["CheckInTime"] = HtmlEncoder.Default.Encode(FormatCheckInTime(model.CheckInTimeAfter));
            tokens["CheckOutTime"] = HtmlEncoder.Default.Encode(FormatCheckOutTime(model.CheckOutTimeBefore));
            tokens["BookingReference"] = HtmlEncoder.Default.Encode(model.BookingReference ?? "N/A");
            tokens["PreHeaderText"] = HtmlEncoder.Default.Encode(
                $"Hi {model.Name}, your stay at {model.PropertyName} is confirmed. " +
                $"Check in {model.CheckIn.ToString("d MMM", CultureInfo.InvariantCulture)} from {FormatCheckInTime(model.CheckInTimeAfter)}.");
            tokens["PropertyUrl"] = string.IsNullOrWhiteSpace(model.PropertyUrl) ? "#" : HtmlEncoder.Default.Encode(model.PropertyUrl);
            tokens["FullAddress"] = HtmlEncoder.Default.Encode(FallbackText(model.FullAddress));
            tokens["DirectionsBlock"] = BuildDirectionsBlock(model.DirectionsUrl);
            tokens["ArrivalInstructionsBlock"] = BuildArrivalInstructionsBlock(model.ArrivalInstructions);
            tokens["HeroImageBlock"] = BuildHeroImageBlock(model.HeroImageUrl, model.HeroImageAltText, model.PropertyName, model.PropertyUrl, model.Bedroom, model.Bathroom);
            tokens["HouseRulesItems"] = BuildTextItemBlock(model.HouseRules);
            tokens["SafetyItems"] = BuildTextItemBlock(model.SafetyItems);
            tokens["CancellationPolicy"] = BuildMultilineParagraphBlock(model.CancellationPolicyText);
            tokens["PaymentRows"] = BuildPaymentRows(model.PaymentLines);

            return tokens;
        }

        private static string ReplaceTokens(string template, IReadOnlyDictionary<string, string> tokens)
        {
            return tokens.Aggregate(template, (current, token) => current.Replace("{{" + token.Key + "}}", token.Value, StringComparison.Ordinal));
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
            if (count is not > 0)
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

        private static string FormatCheckInTime(TimeOnly time) => $"From {time:HH:mm}";

        private static string FormatCheckOutTime(TimeOnly time) => $"Before {time:HH:mm}";


        private static string FallbackText(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "Not provided" : value;

        private static string BuildDirectionsBlock(string? directionsUrl)
        {
            if (string.IsNullOrWhiteSpace(directionsUrl))
            {
                return string.Empty;
            }

            var safeUrl = HtmlEncoder.Default.Encode(directionsUrl);

            return $"""
                <tr>
                  <td style="padding-top:8px;font-size:18px;line-height:28px;">
                    <a href="{safeUrl}" style="color:#222222;font-weight:700;text-decoration:underline;">Get directions</a>
                  </td>
                </tr>
                """;
        }

        private static string BuildArrivalInstructionsBlock(string? arrivalInstructions)
        {
            return string.IsNullOrWhiteSpace(arrivalInstructions) ? string.Empty : $"""
                 <tr>
                   <td style="padding-top:8px;font-size:18px;line-height:28px;color:#222222;">
                     {HtmlEncoder.Default.Encode(arrivalInstructions)}
                   </td>
                 </tr>
                 """;
        }

        private static string BuildHeroImageBlock(string? imageUrl, string? altText, string propertyName, string? propertyUrl, byte? bedroom, double? bathroom)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return string.Empty;
            }

            var safeImageUrl    = HtmlEncoder.Default.Encode(imageUrl);
            var safeAltText     = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(altText) ? propertyName : altText);
            var safePropertyUrl = string.IsNullOrWhiteSpace(propertyUrl) ? null : HtmlEncoder.Default.Encode(propertyUrl);
            var safePropertyName = HtmlEncoder.Default.Encode(propertyName);

            var imageOpen  = safePropertyUrl is null ? string.Empty
                : $"""<a href="{safePropertyUrl}" style="display:block;text-decoration:none;" title="{safeAltText}">""";
            var imageClose = safePropertyUrl is null ? string.Empty : "</a>";

            var imageCell = $"""
                <tr>
                  <td style="padding:0 40px 8px;">
                    {imageOpen}<table role="presentation" style="width:100%;border-collapse:collapse;">
                      <tr>
                        <td background="{safeImageUrl}"
                            aria-label="{safeAltText}"
                            role="img"
                            style="padding-bottom:56.2%;background-position:center;background-color:#F1F1F1;background-repeat:no-repeat !important;background-size:cover !important;height:0px !important;max-height:0px !important;overflow:hidden !important;width:100% !important;border-radius:8px !important;"
                            width="100%" height="0" bgcolor="#F1F1F1">
                        </td>
                      </tr>
                    </table>{imageClose}
                  </td>
                </tr>
                """;

            var nameHtml = safePropertyUrl is null
                ? $"""<p style="margin:0 0 6px;font-size:22px;line-height:28px;font-weight:800;color:#222222;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;">{safePropertyName}</p>"""
                : $"""<p style="margin:0 0 6px;font-size:22px;line-height:28px;font-weight:800;color:#222222;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"><a href="{safePropertyUrl}" style="color:#222222;text-decoration:underline;">{safePropertyName}</a></p>""";

            var metaText = BuildPropertyMetaInline(bedroom, bathroom);
            var metaHtml = string.IsNullOrEmpty(metaText)
                ? string.Empty
                : $"""<p style="margin:0;font-size:17px;line-height:26px;color:#555555;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;">{HtmlEncoder.Default.Encode(metaText)}</p>""";

            var captionRow = $"""
                <tr>
                  <td style="padding:14px 40px 30px;">
                    {nameHtml}{metaHtml}
                  </td>
                </tr>
                """;

            return imageCell + captionRow;
        }

        private static string BuildPropertyMetaInline(byte? bedrooms, double? bathrooms)
        {
            var parts = new List<string>();

            if (bedrooms is > 0)
                parts.Add(bedrooms.Value == 1 ? "1 bedroom" : $"{bedrooms.Value} bedrooms");

            if (bathrooms is not > 0) return parts.Count == 0 ? string.Empty : string.Join(" · ", parts);
            var display = bathrooms.Value == Math.Floor(bathrooms.Value)
                ? $"{(int)bathrooms.Value}"
                : bathrooms.Value.ToString("0.#", CultureInfo.InvariantCulture);
            parts.Add(display == "1" ? "1 bathroom" : $"{display} bathrooms");

            return parts.Count == 0 ? string.Empty : string.Join(" · ", parts);
        }

        private static string BuildTextItemBlock(IReadOnlyList<string> items)
        {
            if (items.Count == 0)
            {
                return """
                    <p style="margin:0;font-size:18px;line-height:28px;color:#222222;">Not provided.</p>
                    """;
            }

            return string.Join(
                string.Empty,
                items.Select(item =>
                    $"""
                    <p style="margin:0 0 8px;font-size:18px;line-height:28px;color:#222222;">{HtmlEncoder.Default.Encode(item)}</p>
                    """));
        }

        private static string BuildMultilineParagraphBlock(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return """
                    <p style="margin:0;font-size:18px;line-height:28px;color:#222222;">Not provided.</p>
                    """;
            }

            var lines = value
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(line =>
                    $"""
                    <p style="margin:0 0 8px;font-size:18px;line-height:28px;color:#222222;">{HtmlEncoder.Default.Encode(line)}</p>
                    """);

            return string.Join(string.Empty, lines);
        }

        private static string BuildPaymentRows(IReadOnlyList<BookingConfirmationPaymentLine> paymentLines)
        {
            if (paymentLines.Count == 0)
            {
                return """
                    <tr>
                      <td style="padding:0 0 8px;font-size:18px;line-height:28px;color:#222222;">Not provided</td>
                      <td align="right" style="padding:0 0 8px;font-size:18px;line-height:28px;color:#222222;">-</td>
                    </tr>
                    """;
            }

            return string.Join(
                string.Empty,
                paymentLines.Select(line =>
                    $"""
                    <tr>
                      <td style="padding:0 0 8px;font-size:18px;line-height:28px;color:#666666;">{HtmlEncoder.Default.Encode(line.Label)}</td>
                      <td align="right" style="padding:0 0 8px;font-size:18px;line-height:28px;color:#222222;">{HtmlEncoder.Default.Encode(line.Amount.ToString("C", CultureInfo.GetCultureInfo("en-GB")))}</td>
                    </tr>
                    """));
        }
    }
}
