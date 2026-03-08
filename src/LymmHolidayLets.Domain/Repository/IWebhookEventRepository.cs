using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IWebhookEventRepository
    {
        WebhookEvent? GetByExternalId(string externalId);
        void Create(WebhookEvent webhookEvent);
        void Update(WebhookEvent webhookEvent);
    }
}
