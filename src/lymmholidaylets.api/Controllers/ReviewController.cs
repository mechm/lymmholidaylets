using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Exposes approved customer reviews for display on the website.
    /// Only reviews that have been explicitly approved in the back office are returned.
    /// Results are cached in memory for 10 minutes to reduce database load.
    /// </summary>
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
        /// Returns all approved customer reviews, ordered as stored in the database.
        /// Results are served from an in-memory cache (TTL: 10 minutes).
        /// If no reviews have been approved yet an empty collection is returned rather than 404,
        /// so the front-end never needs to handle a missing-reviews error state.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ReviewSummary"/> items wrapped in a standard <see cref="ApiResponse{T}"/>.
        /// Returns an empty list when no approved reviews exist.
        /// </returns>
        /// <response code="200">Approved reviews returned (may be an empty list).</response>
        /// <response code="500">An unexpected error occurred while loading reviews.</response>
        [HttpGet("init")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReviewSummary>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetApproved()
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
                return Ok(ApiResponse<IEnumerable<ReviewSummary>>.SuccessResult([]));
            }

            logger.LogDebug("Reviews served successfully.");
            return Ok(ApiResponse<IEnumerable<ReviewSummary>>.SuccessResult(reviews));
        }
    }
}
