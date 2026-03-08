using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using LymmHolidayLets.Api.Models.Checkout;
using LymmHolidayLets.Api.Models;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public sealed class CheckoutController(
        ICheckoutService checkoutService,
        IManageCheckoutSessionService manageCheckoutSessionService,
        ILogger<CheckoutController> logger) : ControllerBase
    {
        /// <summary>
        /// Creates a Stripe checkout session for a property booking.
        /// </summary>
        /// <param name="model">The checkout details including property, dates, and guests.</param>
        /// <returns>A JSON object containing the Stripe Checkout URL.</returns>
        /// <response code="200">Successfully created a checkout session.</response>
        /// <response code="400">Invalid booking data or property unavailable.</response>
        /// <response code="500">Internal server error during session creation.</response>
        [HttpPost("create-checkout-session")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public ActionResult Create([FromBody] CheckoutItemForm model)
        {
            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";
            var (error, result) = checkoutService.Checkout(
                host,
                model.PropertyId,
                model.Checkin,
                model.Checkout,
                model.NumberOfAdults,
                model.NumberOfChildren,
                model.NumberOfInfants);

            if (error != null)
            {
                logger.LogWarning("Checkout error for PropertyId={PropertyId}: {Error}", model.PropertyId, error);
                return BadRequest(ApiResponse<object>.FailureResult(error));
            }

            if (result == null)
            {
                logger.LogError("Null result returned from checkout service for PropertyId={PropertyId}", model.PropertyId);
                return BadRequest(ApiResponse<object>.FailureResult("Please try again later or contact us for further support"));
            }

            // Session cache management belongs at the API boundary, not inside the application service.
            manageCheckoutSessionService.AddUpdateSessionCache(result.SessionId, result.CheckIn, result.CheckOut);

            logger.LogInformation("Checkout session created for PropertyId={PropertyId}", model.PropertyId);
            return Ok(ApiResponse<object>.SuccessResult(new { url = result.SessionUrl }));
        }
    }
}