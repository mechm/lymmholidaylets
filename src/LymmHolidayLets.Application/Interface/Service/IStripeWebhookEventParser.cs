using LymmHolidayLets.Application.Model.Service;

namespace LymmHolidayLets.Application.Interface.Service;

public interface IStripeWebhookEventParser
{
    ParsedStripeWebhookEvent? Parse(string json, string? signature);
}
