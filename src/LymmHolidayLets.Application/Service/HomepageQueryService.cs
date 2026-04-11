using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service;

public sealed class HomepageQueryService(
    IApplicationCache cache,
    IHomepageQuery homepageQuery,
    ILogger<HomepageQueryService> logger) : IHomepageQueryService
{
    private const string HomepageCacheKey = "homepage";

    public async Task<HomepageResult?> GetHomepageDataAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(HomepageCacheKey, out HomepageResult? homepage) && homepage is not null)
        {
            logger.LogDebug("Homepage served from cache");
            return homepage;
        }

        try
        {
            var homepageDetail = await homepageQuery.GetHomePageDetailAsync();

            homepage = new HomepageResult(
                homepageDetail.Reviews
                    .Select(review => new HomepageReviewResult(
                        review.FriendlyName,
                        review.Company,
                        review.Description,
                        review.Name,
                        review.Position,
                        review.DateTimeAdded))
                    .ToList(),
                homepageDetail.Slides
                    .Select(slide => new HomepageSlideshowResult(
                        slide.ImagePath,
                        slide.ImagePathAlt,
                        slide.CaptionTitle,
                        slide.Caption,
                        slide.ShortMobileCaption,
                        slide.Link))
                    .ToList());

            cache.SetAbsolute(HomepageCacheKey, homepage, TimeSpan.FromHours(3));

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
