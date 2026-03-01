using Microsoft.AspNetCore.Mvc;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Infrastructure.ModelBinding;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
	public sealed class StripeWebHookController(IStripeWebhookProcessor webhookProcessor) : ControllerBase
	{
        /// <summary>
        /// Receives and processes asynchronous events from Stripe (e.g., payment success).
        /// </summary>
        /// <param name="webhookRequest">The incoming webhook payload and signature.</param>
        /// <returns>A 200 OK status to acknowledge receipt of the event.</returns>
        /// <response code="200">The event was successfully received and queued for processing.</response>
        /// <response code="400">The event signature was invalid or the payload was malformed.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Index([ModelBinder(typeof(StripeWebhookModelBinder))] StripeWebhookRequest webhookRequest)
		{
            var success = await webhookProcessor.ProcessEventAsync(webhookRequest.Json, webhookRequest.Signature);

            if (!success)
            {
                return BadRequest("Stripe webhook processing failed.");
            }

            return Ok();
		}
    }
}
