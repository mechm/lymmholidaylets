using LymmHolidayLets.Api.Infrastructure.SiteMap;
using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteMapController : Controller
    {
        // GET api/sitemap/index.xml
        [HttpGet("index.xml")]
        public IActionResult Index()
        {
            return new XmlSiteMapIndex();
        }
    }
}
