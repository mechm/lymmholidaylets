using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Application.Interface.Query;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Services
{
    public sealed class HomepageService(
        IMemoryCache cache,
        IHomepageQuery homepageQuery,
        ILogger<HomepageService> logger) : IHomepageService
    {
        private const string HomepageCacheKey = "homepage";

        public async Task<HomepageModel?> GetHomepageDataAsync()
        {
            if (cache.TryGetValue(HomepageCacheKey, out HomepageModel? homepage) && homepage != null)
            {
                logger.LogDebug("Homepage served from cache");
                return homepage;
            }

            try
            {
                var homepageDetail = await homepageQuery.GetHomePageDetailAsync();

                homepage = new HomepageModel(
                    homepageDetail.Reviews.Select(review => new
                        ReviewModel(review.FriendlyName, review.Company, review.Description, review.Name,
                            review.Position, review.DateTimeAdded)),
                    homepageDetail.Slides.Select(slide => new
                        SlideshowModel(slide.ImagePath, slide.ImagePathAlt, slide.CaptionTitle, slide.Caption,
                            slide.ShortMobileCaption, slide.Link)));

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.Normal)
                    .SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddHours(3));
                cache.Set(HomepageCacheKey, homepage, cacheEntryOptions);

                logger.LogInformation("Homepage data loaded and cached");
                return homepage;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load homepage data");
                return null;
            }
        }
    }
}
