using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;

namespace LymmHolidayLets.Domain.Model.WebhookEvent.Entity
{
    public sealed class WebhookEvent : IAggregateRoot
    {
        public WebhookEvent(string externalId, string data, WebhookEventState state, string? processingErrors = null)
        {
            ExternalId = externalId;
            Data = data;
            State = state;
            ProcessingErrors = processingErrors;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public WebhookEvent(int id, string externalId, string data, WebhookEventState state, string? processingErrors, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            ExternalId = externalId;
            Data = data;
            State = state;
            ProcessingErrors = processingErrors;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public int Id { get; init; }
        public string ExternalId { get; init; }
        public string Data { get; init; }
        public WebhookEventState State { get; private set; }
        public string? ProcessingErrors { get; private set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; private set; }

        public void MarkAsProcessing(DateTime? now = null)
        {
            if (State is not WebhookEventState.Pending and not WebhookEventState.Failed)
            {
                throw new InvalidOperationException($"Cannot transition webhook event from {State} to {WebhookEventState.Processing}.");
            }

            State = WebhookEventState.Processing;
            ProcessingErrors = null;
            UpdatedAt = now ?? DateTime.UtcNow;
        }

        public void MarkAsProcessed(DateTime? now = null)
        {
            if (State != WebhookEventState.Processing)
            {
                throw new InvalidOperationException($"Cannot transition webhook event from {State} to {WebhookEventState.Processed}.");
            }

            State = WebhookEventState.Processed;
            UpdatedAt = now ?? DateTime.UtcNow;
        }

        public void MarkAsFailed(string error, DateTime? now = null)
        {
            ProcessingErrors = error;
            State = WebhookEventState.Failed;
            UpdatedAt = now ?? DateTime.UtcNow;
        }
    }
}
