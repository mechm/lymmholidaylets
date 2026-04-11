using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Provides aggregated homepage data for the front-end, including approved reviews
    /// and property slideshow images. Acts as a single initialisation call so the UI
    /// can hydrate in one round-trip.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class HomepageController(
        IHomepageQueryService homepageQueryService,
        ILogger<HomepageController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns all data required to render the homepage, including approved reviews
        /// and property slideshow images.
        /// </summary>
        /// <returns>
        /// A <see cref="HomepageModel"/> wrapped in a standard <see cref="ApiResponse{T}"/>.
        /// </returns>
        /// <response code="200">Homepage data loaded and returned successfully.</response>
        /// <response code="500">An unexpected error occurred while loading homepage data.</response>
        [HttpGet("init")]
        [ProducesResponseType(typeof(HomepageModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var homepage = await homepageQueryService.GetHomepageDataAsync();

            if (homepage is not null)
            {
                return Ok(ApiResponse<HomepageModel>.SuccessResult(new HomepageModel(
                    homepage.Reviews.Select(review => new ReviewModel(
                        review.FriendlyName,
                        review.Company,
                        review.Description,
                        review.Name,
                        review.Position,
                        review.DateTimeAdded)),
                    homepage.Slides.Select(slide => new SlideshowModel(
                        slide.ImagePath,
                        slide.ImagePathAlt,
                        slide.CaptionTitle,
                        slide.Caption,
                        slide.ShortMobileCaption,
                        slide.Link)))));
            }
            
            logger.LogWarning("Failed to load homepage data");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<object>.FailureResult("Failed to load homepage data."));

        }
    }
}
