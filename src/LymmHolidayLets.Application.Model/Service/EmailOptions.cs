namespace LymmHolidayLets.Application.Model.Service
{
    /// <summary>
    /// Configuration options for outbound booking confirmation emails.
    /// Bound from the <c>"Email"</c> section of <c>appsettings.json</c>.
    /// </summary>
    public sealed class EmailOptions
    {
        /// <summary>Display name shown in the To/From fields, e.g. "Lymm Holiday Lets".</summary>
        public required string CompanyName { get; init; }

        /// <summary>The inbox that receives company-side booking confirmation emails.</summary>
        public required string CompanyEmail { get; init; }

        /// <summary>
        /// Optional CC recipients for company-side booking confirmation emails,
        /// keyed by display name. Example: <c>{ "Kath": "kath@example.com" }</c>
        /// </summary>
        public IDictionary<string, string> CcEmails { get; init; } = new Dictionary<string, string>();
    }
}

