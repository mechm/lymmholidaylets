using LymmHolidayLets.Api.Infrastructure.SiteMap;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class SiteMapController(ILogger<SiteMapController> logger) : ControllerBase
    {
        /// <summary>
        /// Returns the sitemap index XML.
        /// </summary>
        [HttpGet("index.xml")]
        [Produces("application/xml")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Index()
        {
            logger.LogInformation("Sitemap index requested");
            return new XmlSiteMapIndex();
        }
    }
}
