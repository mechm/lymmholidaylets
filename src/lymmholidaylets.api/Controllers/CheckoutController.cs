using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using LymmHolidayLets.Api.Models.Checkout;
using LymmHolidayLets.Api.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Handles Stripe Checkout session creation for property bookings.
    /// Coordinates availability checks, pricing, discount calculation, and Stripe session initialisation.
    /// The application workflow also records the session metadata needed for later webhook processing.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class CheckoutController(
        ICheckoutService checkoutService,
        ILogger<CheckoutController> logger) : ControllerBase
    {
        /// <summary>
        /// Creates a Stripe Checkout session for a property booking and returns the hosted payment URL.
        /// Validates availability, calculates the nightly rate and any applicable discounts,
        /// creates (or updates) the Stripe product, then initiates the hosted checkout flow.
        /// </summary>
        /// <param name="model">
        /// The booking request containing property ID, check-in/check-out dates, and guest counts.
        /// All fields are validated by <c>CheckoutItemFormValidator</c> before the service layer is called.
        /// </param>
        /// <param name="cancellationToken">
        /// Propagates notification that the request has been cancelled by the caller.
        /// </param>
        /// <returns>
        /// On success, a JSON object with a <c>url</c> property containing the Stripe-hosted checkout URL.
        /// </returns>
        /// <response code="200">Checkout session created. Follow the returned <c>url</c> to complete payment.</response>
        /// <response code="400">
        /// Booking data failed validation, the property is unavailable for the requested dates,
        /// or no guests were specified.
        /// </response>
        /// <response code="429">Too many checkout attempts from this IP. Maximum 10 per minute per client.</response>
        /// <response code="500">An unexpected error occurred during session creation.</response>
        [HttpPost("create-checkout-session")]
        [EnableRateLimiting("CheckoutSession")]
        [ProducesResponseType(typeof(ApiResponse<CheckoutSessionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CheckoutItemForm model, CancellationToken cancellationToken)
        {
            var response = await checkoutService.CheckoutAsync(
                model.PropertyId,
                model.Checkin,
                model.Checkout,
                model.NumberOfAdults,
                model.NumberOfChildren,
                model.NumberOfInfants,
                cancellationToken);

            if (!response.IsSuccess)
            {
                logger.LogWarning("Checkout error for PropertyId={PropertyId}: {Error}", model.PropertyId, response.Error);
                return BadRequest(ApiResponse<object>.FailureResult(response.Error!));
            }

            logger.LogInformation("Checkout session created for PropertyId={PropertyId}", model.PropertyId);
            return Ok(ApiResponse<CheckoutSessionResponse>.SuccessResult(new CheckoutSessionResponse(response.Result.SessionUrl)));
        }
    }
}
