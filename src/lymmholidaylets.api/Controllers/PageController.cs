using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class PageController(
        IMemoryCache cache,
        ILogger<PageController> logger,
        IPageQuery pageQuery) : ControllerBase
    {
        /// <summary>
        /// Gets page details by alias title.
        /// </summary>
        /// <param name="id">Alias title of the page</param>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(ApiResponse<PageDetail>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("Page detail requested with no alias title");
                return BadRequest(ApiResponse<object>.FailureResult("Missing alias title (id)."));
            }

            var cacheKey = $"page-detail-{id}";

            var page = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                logger.LogInformation("Page cache miss for AliasTitle={AliasTitle}. Fetching from database.", id);
                entry.Priority = CacheItemPriority.Normal;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                return await pageQuery.GetPageByAliasTitleAsync(id);
            });

            if (page is null)
            {
                logger.LogWarning("Page not found for AliasTitle={AliasTitle}", id);
                return NotFound();
            }

            if (!page.Visible)
            {
                logger.LogWarning("Page not visible for AliasTitle={AliasTitle}", id);
                return NotFound();
            }

            logger.LogInformation("Page returned successfully for AliasTitle={AliasTitle}", id);
            return Ok(ApiResponse<PageDetail>.SuccessResult(page));
        }
    }
}