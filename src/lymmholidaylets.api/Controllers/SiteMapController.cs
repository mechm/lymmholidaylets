using LymmHolidayLets.Api.Infrastructure.SiteMap;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    /// <summary>
    /// Serves XML sitemap documents for search engine crawlers.
    /// The sitemap index lists all sub-sitemaps (e.g. pages, properties) so that
    /// crawlers can discover all public URLs on the site without a full crawl.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class SiteMapController(ILogger<SiteMapController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns the sitemap index XML document.
        /// The index points search engine crawlers to all individual sitemaps
        /// (e.g. pages, properties) so every public URL on the site can be discovered.
        /// </summary>
        /// <returns>
        /// An <c>application/xml</c> response containing a <c>&lt;sitemapindex&gt;</c> document.
        /// </returns>
        /// <response code="200">Sitemap index XML returned successfully.</response>
        /// <response code="500">An unexpected error occurred while generating the sitemap.</response>
        [HttpGet("index.xml")]
        [Produces("application/xml")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetIndex()
        {
            logger.LogInformation("Sitemap index requested");
            return new XmlSiteMapIndex();
        }
    }
}
