using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Serves CMS page content keyed by an alias title (URL slug).
    /// Page data is cached for 24 hours; only pages marked as <c>Visible</c>
    /// in the database are exposed to callers.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class PageController(
        ILogger<PageController> logger,
        IPageQueryService pageQueryService) : ControllerBase
    {
        /// <summary>
        /// Gets the content and metadata for a CMS page identified by its alias title (URL slug).
        /// Results are cached in memory for 24 hours to reduce database load.
        /// Returns 404 for pages that do not exist <b>or</b> that exist but are marked as not visible,
        /// so that hidden pages are indistinguishable from missing ones to external callers.
        /// </summary>
        /// <param name="alias">
        /// The URL-friendly alias title for the page, e.g. <c>about-us</c> or <c>contact</c>.
        /// Must not be null or whitespace.
        /// </param>
        /// <returns>
        /// A <see cref="PageDetail"/> wrapped in a standard <see cref="ApiResponse{T}"/>.
        /// </returns>
        /// <response code="200">Page found and returned successfully.</response>
        /// <response code="400">The <paramref name="alias"/> parameter was null or empty.</response>
        /// <response code="404">
        /// No page was found for the given alias, or the page exists but is not visible.
        /// </response>
        /// <response code="500">An unexpected error occurred while retrieving the page.</response>
        [HttpGet("detail/{alias}")]
        [ProducesResponseType(typeof(ApiResponse<PageDetail>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Detail(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                logger.LogWarning("Page detail requested with no alias title");
                return BadRequest(ApiResponse<object>.FailureResult("Missing alias title (id)."));
            }

            var page = await pageQueryService.GetVisiblePageByAliasAsync(alias);

            if (page is null)
            {
                logger.LogWarning("Page not found for AliasTitle={AliasTitle}", alias);
                return NotFound();
            }

            logger.LogInformation("Page returned successfully for AliasTitle={AliasTitle}", alias);
            return Ok(ApiResponse<PageDetail>.SuccessResult(page));
        }
    }
}
