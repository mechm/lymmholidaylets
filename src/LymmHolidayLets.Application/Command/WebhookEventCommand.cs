using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class WebhookEventCommand : IWebhookEventCommand
    {
        private readonly IDapperWebhookEventRepository _repository;

        public WebhookEventCommand(IDapperWebhookEventRepository repository)
        {
            _repository = repository;
        }

        public Application.Model.Command.WebhookEvent? GetByExternalId(string externalId)
        {
            var entity = _repository.GetByExternalId(externalId);
            if (entity == null) return null;

            return new Application.Model.Command.WebhookEvent(entity.ExternalId, entity.Data, (int)entity.State, entity.ProcessingErrors);
        }

        public void Create(string externalId, string data)
        {
            var webhookEvent = new WebhookEvent(externalId, data, WebhookEventState.Pending);
            _repository.Create(webhookEvent);
        }

        public void MarkAsProcessing(string externalId)
        {
            var webhookEvent = _repository.GetByExternalId(externalId);
            if (webhookEvent != null)
            {
                webhookEvent.State = WebhookEventState.Processing;
                webhookEvent.UpdatedAt = DateTime.UtcNow;
                _repository.Update(webhookEvent);
            }
        }

        public void MarkAsProcessed(string externalId)
        {
            var webhookEvent = _repository.GetByExternalId(externalId);
            if (webhookEvent != null)
            {
                webhookEvent.State = WebhookEventState.Processed;
                webhookEvent.UpdatedAt = DateTime.UtcNow;
                _repository.Update(webhookEvent);
            }
        }

        public void MarkAsFailed(string externalId, string error)
        {
            var webhookEvent = _repository.GetByExternalId(externalId);
            if (webhookEvent != null)
            {
                webhookEvent.State = WebhookEventState.Failed;
                webhookEvent.ProcessingErrors = error;
                webhookEvent.UpdatedAt = DateTime.UtcNow;
                _repository.Update(webhookEvent);
            }
        }
    }
}
