namespace LymmHolidayLets.Application.Interface.Service
{
    public interface IStripeWebhookProcessor
    {
        Task<bool> ProcessEventAsync(string json, string? signature);
    }
}
