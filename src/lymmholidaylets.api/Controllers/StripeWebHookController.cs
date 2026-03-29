using Microsoft.AspNetCore.Mvc;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Infrastructure.ModelBinding;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Receives and processes incoming Stripe webhook events.
    /// Every event is HMAC-verified using the Stripe signing secret before processing
    /// to prevent spoofed payloads. Duplicate events are handled idempotently by the
    /// <see cref="IStripeWebhookProcessor"/> — re-delivering the same event is safe.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class StripeWebHookController(IStripeWebhookProcessor webhookProcessor) : ControllerBase
    {
        /// <summary>
        /// Receives a Stripe webhook event, verifies the HMAC signature, and delegates
        /// processing to <see cref="IStripeWebhookProcessor"/>.
        /// Currently handles <c>checkout.session.completed</c> to confirm bookings,
        /// but the processor can be extended for additional event types without changing this endpoint.
        /// </summary>
        /// <remarks>
        /// Stripe requires a 200 OK response as quickly as possible to avoid retries.
        /// Business logic (e.g. sending confirmation emails) is handled asynchronously inside the processor.
        /// Idempotency: the processor checks whether an event has already been applied (via session state)
        /// so replayed events do not create duplicate bookings or send duplicate emails.
        /// </remarks>
        /// <param name="webhookRequest">
        /// The raw JSON body and <c>Stripe-Signature</c> header, bound by <see cref="StripeWebhookModelBinder"/>.
        /// </param>
        /// <returns>200 OK to acknowledge receipt; Stripe will retry on any non-2xx response.</returns>
        /// <response code="200">Event received, verified, and processed (or safely ignored as a duplicate).</response>
        /// <response code="400">
        /// Signature verification failed, the payload was malformed, or a processing error occurred.
        /// Stripe will retry delivery after a back-off period.
        /// </response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Receive([ModelBinder(typeof(StripeWebhookModelBinder))] StripeWebhookRequest webhookRequest)
        {
            var success = await webhookProcessor.ProcessEventAsync(webhookRequest.Json, webhookRequest.Signature);
            if (!success)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Stripe webhook processing failed. The event signature may be invalid or the payload malformed."));
            }

            return Ok();
        }
    }
}
