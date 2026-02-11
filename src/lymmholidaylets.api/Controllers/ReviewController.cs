using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Controllers
{    
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController(IMemoryCache cache, IReviewQuery reviewQuery): ControllerBase
    {
        // GET api/review/init
        [HttpGet("init")]
        public IActionResult Index()
        {
            const string reviewKey = "reviews";
            if (cache.TryGetValue(reviewKey, out IEnumerable<ReviewSummary>? reviews) && reviews !=null)
            {
                return Ok(reviews);
            }

            reviews = reviewQuery.GetAllApprovedReviews();
   
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(10));

            cache.Set(reviewKey, reviews, cacheEntryOptions);

            return Ok(reviews);
        }
    }
}
