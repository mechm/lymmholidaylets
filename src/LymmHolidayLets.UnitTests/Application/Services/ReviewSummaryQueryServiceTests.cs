using FluentAssertions;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class ReviewSummaryQueryServiceTests
{
    private readonly Mock<IReviewQuery> _reviewQuery = new();
    private readonly Mock<ILogger<ReviewSummaryQueryService>> _logger = new();
    private readonly ApplicationMemoryCache _cache = new(new MemoryCache(new MemoryCacheOptions()));

    private ReviewSummaryQueryService CreateSut() => new(_cache, _reviewQuery.Object, _logger.Object);

    [Fact]
    public async Task GetApprovedReviewsAsync_WhenNoReviews_ReturnsEmptyList()
    {
        _reviewQuery.Setup(q => q.GetAllApprovedReviewsAsync())
            .ReturnsAsync((IReadOnlyList<ReviewSummary>?)null);

        var result = await CreateSut().GetApprovedReviewsAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetApprovedReviewsAsync_CachesResultOnSecondCall()
    {
        _reviewQuery.Setup(q => q.GetAllApprovedReviewsAsync())
            .ReturnsAsync(new List<ReviewSummary>());
        var sut = CreateSut();

        await sut.GetApprovedReviewsAsync();
        await sut.GetApprovedReviewsAsync();

        _reviewQuery.Verify(q => q.GetAllApprovedReviewsAsync(), Times.Once);
    }
}
