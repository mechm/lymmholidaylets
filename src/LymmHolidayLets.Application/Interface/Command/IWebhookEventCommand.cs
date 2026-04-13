using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IWebhookEventCommand
    {
        void Create(WebhookEvent webhookEvent);
        void Save(WebhookEvent webhookEvent);
    }
}
