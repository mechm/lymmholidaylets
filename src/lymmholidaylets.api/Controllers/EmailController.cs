using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class EmailController(
        IEmailEnquiryService emailEnquiryService,
        IRecaptchaValidationService recaptchaValidationService,
        ILogger<EmailController> logger) : ControllerBase
    { 
        // POST api/email/index
        [HttpPost]
        [EnableRateLimiting("ContactForm")]
        [RequestSizeLimit(100_000)] // Limit request size to ~100KB
        public async Task<IActionResult> Index([FromBody] EmailEnquiryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Model State Validation (handled by framework attributes)
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                // 2. ReCaptcha Verification
                var recaptchaValid = await recaptchaValidationService.ValidateAsync(request.ReCaptchaToken, cancellationToken);
                
                if (!recaptchaValid)
                {
                    logger.LogWarning("ReCaptcha verification failed for IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Security verification failed. Please try again."
                    });
                }

                // 3. Process Enquiry (with proper async handling)
                var success = await emailEnquiryService.ProcessEnquiryAsync(request, cancellationToken);
                
                if (!success)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to process your enquiry. Please try again later."
                    });
                }

                return Ok(new ApiResponse<object>
                { 
                    Success = true, 
                    Message = "Thank you for sending us an enquiry. We aim to respond within 24 hours." 
                });
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Request was cancelled for email enquiry");
                return StatusCode(408, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Request timeout. Please try again."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error processing contact enquiry");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An internal error occurred. Please try again later."
                });
            }
        }
    }
}
