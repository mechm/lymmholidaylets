using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IDapperWebhookEventRepository
    {
        WebhookEvent? GetByExternalId(string externalId);
        void Create(WebhookEvent webhookEvent);
        void Update(WebhookEvent webhookEvent);
    }
}
