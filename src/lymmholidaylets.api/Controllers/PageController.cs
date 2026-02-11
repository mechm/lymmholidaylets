using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Page;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PageController(IMemoryCache cache,
        LymmHolidayLets.Domain.Interface.ILogger logger, IPageQuery pageQuery) : ControllerBase
    {
        private readonly IPageQuery _pageQuery = pageQuery;
        private readonly LymmHolidayLets.Domain.Interface.ILogger _logger = logger;
        private readonly IMemoryCache _cache = cache;

        // GET api/page/detail/{id}
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Unable to retrieve a page with no alias title.", HttpContext);
                    return BadRequest(new { error = "Missing alias title (id)." });
                }

                var cacheKey = $"page-detail-{id}";

                if (!_cache.TryGetValue(cacheKey, out PageDetail? page))
                {
                    page = _pageQuery.GetPageByAliasTitle(id);

                    if (page is null)
                    {
                        _logger.LogWarning($"Unable to retrieve a page with alias title - {id}.", HttpContext);
                        return NotFound();
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetPriority(CacheItemPriority.NeverRemove)
                        .SetAbsoluteExpiration(DateTimeOffset.MaxValue);

                    _cache.Set(cacheKey, page, cacheEntryOptions);
                }

                if (page is { Visible: true })
                {
                    return Ok(page);
                }

                _logger.LogWarning($"Unable to retrieve a visible page with alias title - {id}.", HttpContext);
                return NotFound();
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error retrieving a page with alias title - {id}.", HttpContext, null, ex);
                return StatusCode(500);
            }
        }
    }
}