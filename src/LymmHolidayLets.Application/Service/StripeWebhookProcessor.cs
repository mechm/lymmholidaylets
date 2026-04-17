using System.Globalization;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using DomainWebhookEvent = LymmHolidayLets.Domain.Model.WebhookEvent.Entity.WebhookEvent;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;
using LymmHolidayLets.Domain.ReadModel.Property;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service
{
    public sealed class StripeWebhookProcessor(
        IConfiguration config,
        ILogger<StripeWebhookProcessor> logger,
        IApplicationCache cache,
        IStripeWebhookEventParser stripeWebhookEventParser,
        IPropertyQuery propertyQuery,
        IPublishEndpoint publishEndpoint,
        IManageCheckoutSessionService manageCheckoutSessionService,
        IStripeService stripeService,
        IBookingCommand bookingCommand,
        IWebhookEventQuery webhookEventQuery,
        IWebhookEventCommand webhookEventCommand) : IStripeWebhookProcessor
    {
        public async Task<bool> ProcessEventAsync(string json, string? signature)
        {
            DomainWebhookEvent? webhookEvent = null;

            try
            {
                var stripeEvent = stripeWebhookEventParser.Parse(json, signature);
                if (stripeEvent is null)
                {
                    logger.LogWarning("Stripe webhook payload could not be parsed.");
                    return false;
                }

                // Idempotency check: Stripe may send the same event multiple times if it doesn't receive a 200 OK.
                // We track event IDs in the database to ensures we don't process the same booking or payment twice.
                webhookEvent = webhookEventQuery.GetByExternalId(stripeEvent.EventId);
                if (webhookEvent != null)
                {
                    // If the event is already processed or currently being handled, we return true (200 OK)
                    // so Stripe stops retrying, while avoiding duplicate side effects.
                    if (webhookEvent.State is WebhookEventState.Processed or WebhookEventState.Processing)
                    {
                        return true; 
                    }
                }
                else
                {
                    // First time seeing this event, record it so subsequent retries are caught.
                    webhookEvent = new DomainWebhookEvent(stripeEvent.EventId, json, WebhookEventState.Pending);
                    webhookEventCommand.Create(webhookEvent);
                }

                webhookEvent.MarkAsProcessing();
                webhookEventCommand.Save(webhookEvent);

                switch (stripeEvent.EventType)
                {
                    case "checkout.session.completed":
                        await HandleCheckoutSessionCompleted(stripeEvent, webhookEvent);
                        break;

                    default:
                        logger.LogWarning("Unhandled event type: {EventType}", stripeEvent.EventType);
                        webhookEvent.MarkAsProcessed();
                        webhookEventCommand.Save(webhookEvent);
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StripeWebhookProcessor");
                if (webhookEvent == null)
                {
                    return false;
                }
                webhookEvent.MarkAsFailed(ex.Message);
                webhookEventCommand.Save(webhookEvent);
                return false;
            }
        }

        private async Task HandleCheckoutSessionCompleted(ParsedStripeWebhookEvent stripeEvent, DomainWebhookEvent webhookEvent)
        {
            if (stripeEvent.CheckoutSession is not { PaymentStatus: "paid" } session)
            {
                webhookEvent.MarkAsProcessed();
                webhookEventCommand.Save(webhookEvent);
                return;
            }

            if (!session.Metadata.TryGetValue("PropertyID", out var propertyIdStr) ||
                !session.Metadata.TryGetValue("CheckInDate", out var checkInDateStr) ||
                !session.Metadata.TryGetValue("CheckoutDate", out var checkoutDateStr) ||
                !session.Metadata.TryGetValue("PropertyName", out var propertyName))
            {
                throw new Exception("Missing metadata fields in Stripe session.");
            }

            if (!byte.TryParse(propertyIdStr, out var propertyId))
            {
                throw new Exception("Invalid PropertyID format.");
            }

            PropertyCheckInCheckOutTime? checkinCheckOut = propertyQuery.GetPropertyCheckInCheckOutTime(propertyId);
            if (checkinCheckOut == null)
            {
                throw new Exception($"Property with ID {propertyId} not found.");
            }

            byte? noAdult = session.Metadata.TryGetValue("NoAdult", out string? noA) && !string.IsNullOrEmpty(noA) ? Convert.ToByte(noA) : null;
            byte? noChildren = session.Metadata.TryGetValue("NoChildren", out string? noC) && !string.IsNullOrEmpty(noC) ? Convert.ToByte(noC) : null;
            byte? noInfant = session.Metadata.TryGetValue("NoInfant", out string? noI) && !string.IsNullOrEmpty(noI) ? Convert.ToByte(noI) : null;

            DateOnly checkIn = DateOnly.ParseExact(checkInDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateOnly checkout = DateOnly.ParseExact(checkoutDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            DateTime checkInUtc = DateTime.SpecifyKind(checkIn.ToDateTime(checkinCheckOut.CheckInTimeAfter), DateTimeKind.Utc);
            DateTime checkoutUtc = DateTime.SpecifyKind(checkout.ToDateTime(checkinCheckOut.CheckOutTimeBefore), DateTimeKind.Utc);

            // Execute logical tasks
            await UpdateSessionsAsync(checkIn, checkout);
            UpdateBooking(session, stripeEvent.EventId, propertyId, checkInUtc, checkoutUtc, noAdult, noChildren, noInfant);

            cache.Remove($"ical-availability-{propertyId}");
            cache.Remove($"property-detail-{propertyId}");

            // Publish notification event - NotificationWorker will handle email + SMS
            await SendNotifications(session, propertyId, propertyName, checkIn, checkout, noAdult, noChildren, noInfant);

            webhookEvent.MarkAsProcessed();
            webhookEventCommand.Save(webhookEvent);
        }

        private async Task UpdateSessionsAsync(DateOnly checkIn, DateOnly checkout)
        {
            IList<CheckoutSession> currentSessions = manageCheckoutSessionService.GetCurrentSessions();

            IList<CheckoutSession> checkoutSessions = manageCheckoutSessionService.GetSessionsBasedOnDates(currentSessions, checkIn, checkout);

            foreach (CheckoutSession checkoutSession in checkoutSessions)
            {
                if (!(checkoutSession.CheckIn == checkIn && checkoutSession.Checkout == checkout))
                {
                    await stripeService.ExpireSessionAsync(checkoutSession.SessionId);
                }
                currentSessions.Remove(checkoutSession);
            }
            manageCheckoutSessionService.UpdateSessionCache(currentSessions);
        }

        private void UpdateBooking(ParsedStripeCheckoutSession session, string stripeEventId, byte propertyId, DateTime checkIn, DateTime checkout, byte? noAdult, byte? noChildren, byte? noInfant)
        {
            bookingCommand.Create(new Application.Model.Command.Booking(stripeEventId, session.SessionId,
                propertyId, checkIn,
                checkout, noAdult, noChildren, noInfant,
                session.CustomerDetails.Name, session.CustomerDetails.Email,
                session.CustomerDetails.Phone, session.CustomerDetails.PostalCode,
                session.CustomerDetails.Country, session.AmountTotal));
        }

        private async Task SendNotifications(ParsedStripeCheckoutSession session, byte propertyId, string propertyName, DateOnly checkIn, DateOnly checkout, byte? noAdult, byte? noChildren, byte? noInfant)
        {
            try
            {
                string[] smsRecipients = config["Keys:Telephone"]?.Split("|") ?? [];

                // Publish single event for all notifications - NotificationWorker handles the rest
                await publishEndpoint.Publish(new BookingNotificationRequested(
                    propertyName, checkIn, checkout,
                    noAdult, noChildren, noInfant,
                    session.CustomerDetails.Name,
                    session.CustomerDetails.Email,
                    session.CustomerDetails.Phone,
                    session.CustomerDetails.PostalCode,
                    session.CustomerDetails.Country,
                    session.AmountTotal,
                    propertyId,
                    smsRecipients,
                    session.SessionId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error publishing booking notification event");
            }
        }
    }
}
