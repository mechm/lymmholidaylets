using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IWebhookEventQuery
    {
        WebhookEvent? GetByExternalId(string externalId);
    }
}
