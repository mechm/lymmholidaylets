using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class WebhookEventQuery(IWebhookEventRepository repository) : IWebhookEventQuery
    {
        public WebhookEvent? GetByExternalId(string externalId) => repository.GetByExternalId(externalId);
    }
}
