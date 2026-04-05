using Asp.Versioning;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Property;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Provides REST endpoints for property detail and listing data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class PropertyController(
        IMemoryCache cache,
        ILogger<PropertyController> logger,
        IPropertyQuery propertyQuery,
        ISocialShareLinkGenerator socialShareLinkGenerator) : ControllerBase
    {
        /// <summary>
        /// Gets full detail for a property including booking capacity, booked dates,
        /// FAQs, aggregated guest reviews, host information, map coordinates, and social sharing links.
        /// Results are cached for 10 minutes; the cache entry is evictable under memory pressure.
        /// </summary>
        /// <param name="id">The numeric property identifier (e.g. <c>1</c>).</param>
        /// <returns>
        /// A <see cref="PropertyDetailResponse"/> wrapped in a standard <see cref="ApiResponse{T}"/>.
        /// </returns>
        /// <response code="200">Property detail returned successfully.</response>
        /// <response code="404">No property was found for the given id.</response>
        /// <response code="500">An unexpected error occurred while retrieving the property.</response>
        [HttpGet("detail/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Detail(byte id)
        {
            var cacheKey = $"property-detail-{id}";

            if (!cache.TryGetValue(cacheKey, out PropertyDetailResult? detail))
            {
                logger.LogInformation(
                    "Property detail cache miss for PropertyId={PropertyId}. Fetching from database.", id);

                detail = await propertyQuery.GetPropertyDetailByIdAsync(id);

                if (detail is not null)
                {
                    cache.Set(cacheKey, detail, new MemoryCacheEntryOptions
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
                }
            }

            if (detail is null)
            {
                logger.LogWarning("Property not found for PropertyId={PropertyId}", id);
                return NotFound();
            }

            // Generate social sharing links from current HTTP context
            var shareLinks = socialShareLinkGenerator.GenerateLinks(id, detail.DisplayAddress);

            var response = new PropertyDetailResponse
            {
                PropertyDetail = detail,
                PropertyUrl = shareLinks.PropertyUrl,
                FacebookShareLink = shareLinks.FacebookShareLink,
                TwitterShareLink = shareLinks.TwitterShareLink,
                LinkedInShareLink = shareLinks.LinkedInShareLink,
                EmailShareLink = shareLinks.EmailShareLink,
                Reviews = detail.ReviewAggregate?.Reviews
                    .Select(ReviewResponse.FromApplicationModel)
                    .ToList()
            };

            logger.LogDebug("Property detail served for PropertyId={PropertyId}", id);
            return Ok(ApiResponse<PropertyDetailResponse>.SuccessResult(response));
        }
    }
}
