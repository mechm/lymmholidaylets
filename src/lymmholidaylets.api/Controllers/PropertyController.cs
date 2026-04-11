using Asp.Versioning;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Property;
using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Provides REST endpoints for property detail and listing data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class PropertyController(
        ILogger<PropertyController> logger,
        IPropertyDetailQueryService propertyDetailQueryService,
        IPropertyDetailResponseBuilder responseBuilder) : ControllerBase
    {
        /// <summary>
        /// Gets full detail for a property including booking capacity, booked dates,
        /// FAQs, aggregated guest reviews, host information, map coordinates, and social sharing links.
        /// Results are cached for 1 hour; the cache entry is evicted immediately when a review or FAQ
        /// is created, updated, or deleted via the respective commands.
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
            var detail = await propertyDetailQueryService.GetPropertyDetailAsync(id);

            if (detail is null)
            {
                logger.LogWarning("Property not found for PropertyId={PropertyId}", id);
                return NotFound();
            }

            if (detail.LastModified.HasValue)
                Response.GetTypedHeaders().LastModified = detail.LastModified.Value;

            logger.LogDebug("Property detail served for PropertyId={PropertyId}", id);
            return Ok(ApiResponse<PropertyDetailResponse>.SuccessResult(responseBuilder.Build(detail)));
        }
    }
}
