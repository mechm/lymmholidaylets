using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class WebhookEventCommand(IWebhookEventRepository repository) : IWebhookEventCommand
    {
        public void Create(Domain.Model.WebhookEvent.Entity.WebhookEvent webhookEvent)
        {
            repository.Create(webhookEvent);
        }

        public void Save(Domain.Model.WebhookEvent.Entity.WebhookEvent webhookEvent)
        {
            repository.Update(webhookEvent);
        }
    }
}
