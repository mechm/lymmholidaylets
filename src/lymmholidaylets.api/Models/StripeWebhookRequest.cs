namespace LymmHolidayLets.Api.Models
{
    public sealed class StripeWebhookRequest
    {
        public string Json { get; init; } = string.Empty;
        public string Signature { get; init; } = string.Empty;
    }
}
