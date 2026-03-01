using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IWebhookEventCommand
    {
        WebhookEvent? GetByExternalId(string externalId);
        void Create(string externalId, string data);
        void MarkAsProcessing(string externalId);
        void MarkAsProcessed(string externalId);
        void MarkAsFailed(string externalId, string error);
    }
}
