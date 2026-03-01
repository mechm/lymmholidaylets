using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class HomepageController(
        IHomepageService homepageService,
        ILogger<HomepageController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns homepage data including reviews and slideshow images.
        /// </summary>
        [HttpGet("init")]
        [ProducesResponseType(typeof(HomepageModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Index()
        {
            var homepage = await homepageService.GetHomepageDataAsync();

            if (homepage == null)
            {
                logger.LogWarning("Failed to load homepage data");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse<object>.FailureResult("Failed to load homepage data."));
            }

            return Ok(ApiResponse<HomepageModel>.SuccessResult(homepage));
        }
    }
}
