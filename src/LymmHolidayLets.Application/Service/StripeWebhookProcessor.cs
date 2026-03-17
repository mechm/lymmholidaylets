using Stripe;
using Stripe.Checkout;
using System.Globalization;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;
using LymmHolidayLets.Domain.ReadModel.Property;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service
{
    public sealed class StripeWebhookProcessor(
        IConfiguration config,
        ILogger<StripeWebhookProcessor> logger,
        IMemoryCache cache,
        IPropertyQuery propertyQuery,
        IEmailGeneratorService emailGeneratorService,
        IManageCheckoutSessionService manageCheckoutSessionService,
        IStripeService stripeService,
        ITextMessageService textMessageService,
        IBookingCommand bookingCommand,
        IWebhookEventCommand webhookEventCommand) : IStripeWebhookProcessor
    {
        public async Task<bool> ProcessEventAsync(string json, string? signature)
        {
            Event? stripeEvent = null;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signature, config["StripeSettings:CheckoutWebHookKey"]);

                // Idempotency check: Stripe may send the same event multiple times if it doesn't receive a 200 OK.
                // We track event IDs in the database to ensures we don't process the same booking or payment twice.
                var existingEvent = webhookEventCommand.GetByExternalId(stripeEvent.Id);
                if (existingEvent != null)
                {
                    // If the event is already processed or currently being handled, we return true (200 OK)
                    // so Stripe stops retrying, while avoiding duplicate side effects.
                    if (existingEvent.State is (int)WebhookEventState.Processed or (int)WebhookEventState.Processing)
                    {
                        return true; 
                    }
                }
                else
                {
                    // First time seeing this event, record it so subsequent retries are caught.
                    webhookEventCommand.Create(stripeEvent.Id, json);
                }

                webhookEventCommand.MarkAsProcessing(stripeEvent.Id);

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        break;

                    default:
                        logger.LogWarning("Unhandled event type: {EventType}", stripeEvent.Type);
                        webhookEventCommand.MarkAsProcessed(stripeEvent.Id);
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StripeWebhookProcessor");
                if (stripeEvent != null)
                {
                    webhookEventCommand.MarkAsFailed(stripeEvent.Id, ex.Message);
                }
                return false;
            }
        }

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            if (stripeEvent.Data.Object is not Session { PaymentStatus: "paid" } session) return;

            if (!session.Metadata.TryGetValue("PropertyID", out string? propertyIdStr) ||
                !session.Metadata.TryGetValue("CheckInDate", out string? checkInDateStr) ||
                !session.Metadata.TryGetValue("CheckoutDate", out string? checkoutDateStr) ||
                !session.Metadata.TryGetValue("PropertyName", out string? propertyName))
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
            UpdateBooking(session, stripeEvent.Id, propertyId, checkInUtc, checkoutUtc, noAdult, noChildren, noInfant);

            cache.Remove($"ical-availability-{propertyId}");
            //cache.Remove($"property-detail-{propertyId}");

            // Handle notifications
            await SendNotifications(session, propertyName, checkIn, checkout, noAdult, noChildren, noInfant);

            webhookEventCommand.MarkAsProcessed(stripeEvent.Id);
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

        private void UpdateBooking(Session session, string stripeEventId, byte propertyId, DateTime checkIn, DateTime checkout, byte? noAdult, byte? noChildren, byte? noInfant)
        {
            bookingCommand.Create(new Application.Model.Command.Booking(stripeEventId, session.Id,
                propertyId, checkIn,
                checkout, noAdult, noChildren, noInfant,
                session.CustomerDetails.Name, session.CustomerDetails.Email,
                session.CustomerDetails.Phone, session.CustomerDetails.Address.PostalCode,
                session.CustomerDetails.Address.Country, session.AmountTotal));
        }

        private async Task SendNotifications(Session session, string propertyName, DateOnly checkIn, DateOnly checkout, byte? noAdult, byte? noChildren, byte? noInfant)
        {
            try
            {
                string[] multiNumbers = config["Keys:Telephone"]?.Split("|") ?? [];
                decimal? amount = session.AmountTotal / 100M;
                string price = amount % 1 > 0 ? $"{amount:C2}" : $"{amount:C0}";
                string message = $"{session.CustomerDetails.Name} booked {propertyName} {checkIn:dd/MM/yyyy} to {checkout:dd/MM/yyyy} for {price}, " +
                                 $"telephone: {session.CustomerDetails.Phone}, email: {session.CustomerDetails.Email}";

                var textTask = textMessageService.SendText(message, multiNumbers);

                var companyEmailTask = emailGeneratorService.EmailBookingConfirmationToCompany(
                        new BookingConfirmationForCompany(propertyName, checkIn, checkout, noAdult, noChildren, noInfant,
                            session.CustomerDetails.Name, session.CustomerDetails.Email,
                            session.CustomerDetails.Phone, session.CustomerDetails.Address.PostalCode,
                            session.CustomerDetails.Address.Country, session.AmountTotal));

                var customerEmailTask = emailGeneratorService.EmailBookingConfirmationToCustomer(
                    new BookingConfirmationForCustomer(propertyName, checkIn, checkout, noAdult, noChildren, noInfant,
                        session.CustomerDetails.Name, session.CustomerDetails.Email,
                        session.CustomerDetails.Phone, session.CustomerDetails.Address.PostalCode,
                        session.CustomerDetails.Address.Country, session.AmountTotal
                    ));

                await Task.WhenAll(textTask, companyEmailTask, customerEmailTask);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending booking notifications");
            }
        }
    }
}
