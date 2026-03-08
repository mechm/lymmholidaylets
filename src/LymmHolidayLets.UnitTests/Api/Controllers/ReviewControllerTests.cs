using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class ReviewControllerTests
{
    private readonly Mock<IReviewQuery> _reviewQuery = new();
    private readonly Mock<ILogger<ReviewController>> _logger = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private ReviewController CreateSut() => new(_cache, _logger.Object, _reviewQuery.Object);

    [Fact]
    public async Task Index_WhenNoReviews_ReturnsOkWithEmptyList()
    {
        _reviewQuery.Setup(q => q.GetAllApprovedReviewsAsync())
            .ReturnsAsync((IReadOnlyList<ReviewSummary>?)null);

        var result = await CreateSut().Index();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<IEnumerable<ReviewSummary>>>().Subject;
        body.Success.Should().BeTrue();
        body.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Index_WhenReviewsExist_ReturnsOkWithReviews()
    {
        IReadOnlyList<ReviewSummary> reviews = new List<ReviewSummary>
        {
            new() { PropertyId = 1, PropertyName = "Lymm", Name = "John", ReviewType = "Google", Description = "Great!" }
        };
        _reviewQuery.Setup(q => q.GetAllApprovedReviewsAsync()).ReturnsAsync(reviews);

        var result = await CreateSut().Index();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<IEnumerable<ReviewSummary>>>().Subject;
        body.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Index_CachesResultOnSecondCall()
    {
        _reviewQuery.Setup(q => q.GetAllApprovedReviewsAsync())
            .ReturnsAsync(new List<ReviewSummary>() as IReadOnlyList<ReviewSummary>);
        var sut = CreateSut();

        await sut.Index();
        await sut.Index();

        _reviewQuery.Verify(q => q.GetAllApprovedReviewsAsync(), Times.Once);
    }
}
