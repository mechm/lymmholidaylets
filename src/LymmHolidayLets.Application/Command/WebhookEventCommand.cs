using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class WebhookEventCommand(IWebhookEventRepository repository) : IWebhookEventCommand
    {
        public Application.Model.Command.WebhookEvent? GetByExternalId(string externalId)
        {
            var entity = repository.GetByExternalId(externalId);
            return entity == null ? null : 
                new Application.Model.Command.WebhookEvent(entity.ExternalId, entity.Data, (int)entity.State, entity.ProcessingErrors);
        }

        public void Create(string externalId, string data)
        {
            var webhookEvent = new WebhookEvent(externalId, data, WebhookEventState.Pending);
            repository.Create(webhookEvent);
        }

        public void MarkAsProcessing(string externalId)
        {
            var webhookEvent = repository.GetByExternalId(externalId);
            if (webhookEvent == null)
            {
                return;
            }
            webhookEvent.State = WebhookEventState.Processing;
            webhookEvent.UpdatedAt = DateTime.UtcNow;
            repository.Update(webhookEvent);
        }

        public void MarkAsProcessed(string externalId)
        {
            var webhookEvent = repository.GetByExternalId(externalId);
            if (webhookEvent == null)
            {
                return;
            }
            webhookEvent.State = WebhookEventState.Processed;
            webhookEvent.UpdatedAt = DateTime.UtcNow;
            repository.Update(webhookEvent);
        }

        public void MarkAsFailed(string externalId, string error)
        {
            var webhookEvent = repository.GetByExternalId(externalId);
            if (webhookEvent == null)
            {
                return;
            }
            webhookEvent.State = WebhookEventState.Failed;
            webhookEvent.ProcessingErrors = error;
            webhookEvent.UpdatedAt = DateTime.UtcNow;
            repository.Update(webhookEvent);
        }
    }
}
