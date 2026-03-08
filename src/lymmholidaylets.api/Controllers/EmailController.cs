using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
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
        /// <response code="408">The request timed out.</response>
        /// <response code="500">Internal server error during email processing.</response>
        [HttpPost]
        [EnableRateLimiting("ContactForm")]
        [RequestSizeLimit(100_000)] // Limit request size to ~100KB
        public async Task<IActionResult> Index([FromBody] EmailEnquiryRequest request, CancellationToken cancellationToken)
        {
            // Use structured logging for all log messages
            var logPayload = new
            {
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                RequestEmail = request.EmailAddress,
                RequestName = request.Name
            };

            // 1. Model State Validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                logger.LogWarning("Email enquiry validation failed for {RequestEmail}: {Errors}", request.EmailAddress, errors);
                return BadRequest(ApiResponse<object>.FailureResult("Validation failed", errors));
            }

            // 2. ReCaptcha Verification
            var recaptchaValid = await recaptchaValidationService.ValidateAsync(request.ReCaptchaToken, cancellationToken);
            
            if (!recaptchaValid)
            {
                logger.LogWarning("ReCaptcha verification failed for {RequestEmail}", logPayload);
                return BadRequest(ApiResponse<object>.FailureResult("Security verification failed. Please try again."));
            }

            // 3. Process Enquiry
            var success = await emailEnquiryService.ProcessEnquiryAsync(request, cancellationToken);
            
            if (!success)
            {
                logger.LogError("Email enquiry processing failed for {RequestEmail}", logPayload);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse<object>.FailureResult("Failed to process your enquiry. Please try again later."));
            }

            logger.LogInformation("Email enquiry successfully processed for {RequestEmail}", logPayload);
            return Ok(ApiResponse<object>.SuccessResult(null, "Thank you for sending us an enquiry. We aim to respond within 24 hours."));
        }
    }
}
