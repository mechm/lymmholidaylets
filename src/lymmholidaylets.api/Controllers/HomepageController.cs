using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Application.Interface.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class HomepageController(IMemoryCache cache, IHomepageQuery homepageQuery) : ControllerBase
    {
        // GET api/homepage/init
        [HttpGet("init")]
        public IActionResult Index()
        {
            const string homepageKey = "homepage";

            if (cache.TryGetValue(homepageKey, out HomepageModel? homepage) && homepage != null)
            {
                return Ok(homepage);
            }

            var homepageDetail = homepageQuery.GetHomePageDetail();

            homepage = new HomepageModel(
                homepageDetail.Reviews.Select(review => new
                    ReviewModel(review.FriendlyName, review.Company, review.Description, review.Name,
                        review.Position, review.DateTimeAdded)),
                homepageDetail.Slides.Select(slide => new
                    SlideshowModel(slide.ImagePath, slide.ImagePathAlt, slide.CaptionTitle, slide.Caption,
                        slide.ShortMobileCaption, slide.Link)));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(3));
            cache.Set(homepageKey, homepage, cacheEntryOptions);

            return Ok(homepage);
        }
    }
}