using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Handles customer contact-form email enquiries.
    /// Every submission is rate-limited and reCAPTCHA-verified before the enquiry
    /// is forwarded to the property owners via the email service.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class EmailController(
        IEmailEnquiryService emailEnquiryService,
        IRecaptchaValidationService recaptchaValidationService,
        ILogger<EmailController> logger) : ControllerBase
    {
        /// <summary>
        /// Processes a customer email enquiry from the contact form.
        /// </summary>
        /// <param name="request">The enquiry request details including name, email, and message.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>A standard API response indicating success or failure.</returns>
        /// <response code="200">Enquiry successfully processed and email sent.</response>
        /// <response code="400">Validation failed or security check (reCaptcha) failed.</response>
        /// <response code="429">Too many submissions. Maximum 5 per minute per client.</response>
        /// <response code="500">Internal server error during email processing.</response>
        [HttpPost]
        [EnableRateLimiting("ContactForm")]
        [RequestSizeLimit(100_000)] // Limit request size to ~100KB
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Submit([FromBody] EmailEnquiryRequest request, CancellationToken cancellationToken)
        {
            var logPayload = new
            {
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                RequestEmail = request.EmailAddress,
                RequestName = request.Name
            };

            // ReCaptcha Verification — performed after model binding so we only
            // hit the reCaptcha service when the request shape is already valid.
            var recaptchaValid = await recaptchaValidationService.ValidateAsync(request.ReCaptchaToken, cancellationToken);
            if (!recaptchaValid)
            {
                logger.LogWarning("ReCaptcha verification failed {@LogPayload}", logPayload);
                return BadRequest(ApiResponse<object>.FailureResult("Security verification failed. Please try again."));
            }

            // Process Enquiry
            var success = await emailEnquiryService.ProcessEnquiryAsync(request, cancellationToken);
            if (!success)
            {
                logger.LogError("Email enquiry processing failed {@LogPayload}", logPayload);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse<object>.FailureResult("Failed to process your enquiry. Please try again later."));
            }

            logger.LogInformation("Email enquiry successfully processed {@LogPayload}", logPayload);
            return Ok(ApiResponse<object>.SuccessResult(null, "Thank you for sending us an enquiry. We aim to respond within 24 hours."));
        }
    }
}
