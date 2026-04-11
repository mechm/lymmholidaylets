using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class ReviewControllerTests
{
    private readonly Mock<IReviewSummaryQueryService> _reviewSummaryQueryService = new();
    private readonly Mock<ILogger<ReviewController>> _logger = new();

    private ReviewController CreateSut() => new(_logger.Object, _reviewSummaryQueryService.Object);

    [Fact]
    public async Task GetApproved_WhenNoReviews_ReturnsOkWithEmptyList()
    {
        _reviewSummaryQueryService.Setup(q => q.GetApprovedReviewsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReviewSummary>?)null);

        var result = await CreateSut().GetApproved();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<IEnumerable<ReviewSummary>>>().Subject;
        body.Success.Should().BeTrue();
        body.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetApproved_WhenReviewsExist_ReturnsOkWithReviews()
    {
        IReadOnlyList<ReviewSummary> reviews = new List<ReviewSummary>
        {
            new() { PropertyId = 1, PropertyName = "Lymm", Name = "John", ReviewType = "Google", Description = "Great!" }
        };
        _reviewSummaryQueryService.Setup(q => q.GetApprovedReviewsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reviews);

        var result = await CreateSut().GetApproved();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<IEnumerable<ReviewSummary>>>().Subject;
        body.Data.Should().HaveCount(1);
    }

}
