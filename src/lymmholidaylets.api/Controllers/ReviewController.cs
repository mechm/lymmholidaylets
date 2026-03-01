using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class ReviewController(
        IMemoryCache cache,
        ILogger<ReviewController> logger,
        IReviewQuery reviewQuery) : ControllerBase
    {
        private const string ReviewsCacheKey = "reviews";

        /// <summary>
        /// Gets all approved reviews.
        /// </summary>
        [HttpGet("init")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReviewSummary>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ReviewSummary>>> Index()
        {
            var reviews = await cache.GetOrCreateAsync(ReviewsCacheKey, async entry =>
            {
                logger.LogInformation("Reviews cache miss. Fetching from database.");
                entry.Priority = CacheItemPriority.Normal;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await reviewQuery.GetAllApprovedReviewsAsync();
            });

            if (reviews is null)
            {
                logger.LogWarning("No reviews found.");
                return Ok(ApiResponse<IEnumerable<ReviewSummary>>.SuccessResult(new List<ReviewSummary>()));
            }

            logger.LogDebug("Reviews served successfully.");
            return Ok(ApiResponse<IEnumerable<ReviewSummary>>.SuccessResult(reviews));
        }
    }
}
