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
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>A JSON object containing the Stripe Checkout URL.</returns>
        /// <response code="200">Successfully created a checkout session.</response>
        /// <response code="400">Invalid booking data or property unavailable.</response>
        /// <response code="500">Internal server error during session creation.</response>
        [HttpPost("create-checkout-session")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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

            // Session cache management belongs at the API boundary, not inside the application service.
            manageCheckoutSessionService.AddUpdateSessionCache(response.Result!.SessionId, response.Result.CheckIn, response.Result.CheckOut);

            logger.LogInformation("Checkout session created for PropertyId={PropertyId}", model.PropertyId);
            return Ok(ApiResponse<object>.SuccessResult(new { url = response.Result.SessionUrl }));
        }
    }
}