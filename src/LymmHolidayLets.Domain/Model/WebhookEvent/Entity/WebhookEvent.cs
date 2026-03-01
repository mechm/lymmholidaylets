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
        public WebhookEventState State { get; set; }
        public string? ProcessingErrors { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
    }
}
