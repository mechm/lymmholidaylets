using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using StripeEvent = Stripe.Event;

namespace LymmHolidayLets.Infrastructure.Services;

public sealed class StripeWebhookEventParser(IConfiguration configuration) : IStripeWebhookEventParser
{
    public ParsedStripeWebhookEvent? Parse(string json, string? signature)
    {
        StripeEvent stripeEvent = EventUtility.ConstructEvent(
            json,
            signature,
            configuration["StripeSettings:CheckoutWebHookKey"]);

        return new ParsedStripeWebhookEvent
        {
            EventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            CheckoutSession = stripeEvent.Data.Object is Session session
                ? new ParsedStripeCheckoutSession
                {
                    SessionId = session.Id,
                    PaymentStatus = session.PaymentStatus,
                    Metadata = new Dictionary<string, string>(session.Metadata),
                    AmountTotal = session.AmountTotal,
                    CustomerDetails = new ParsedStripeCustomerDetails
                    {
                        Name = session.CustomerDetails?.Name,
                        Email = session.CustomerDetails?.Email,
                        Phone = session.CustomerDetails?.Phone,
                        PostalCode = session.CustomerDetails?.Address?.PostalCode,
                        Country = session.CustomerDetails?.Address?.Country
                    }
                }
                : null
        };
    }
}
