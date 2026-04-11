using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Review;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service;

public sealed class ReviewSummaryQueryService(
    IApplicationCache cache,
    IReviewQuery reviewQuery,
    ILogger<ReviewSummaryQueryService> logger) : IReviewSummaryQueryService
{
    private const string ReviewsCacheKey = "reviews";

    public async Task<IReadOnlyList<ReviewSummary>> GetApprovedReviewsAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(ReviewsCacheKey, out IReadOnlyList<ReviewSummary>? reviews) && reviews is not null)
        {
            return reviews;
        }

        logger.LogInformation("Reviews cache miss. Fetching from database.");
        reviews = await reviewQuery.GetAllApprovedReviewsAsync() ?? [];
        cache.SetAbsolute(ReviewsCacheKey, reviews, TimeSpan.FromMinutes(10));
        return reviews;
    }
}
