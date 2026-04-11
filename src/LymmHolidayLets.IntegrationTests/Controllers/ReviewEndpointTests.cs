using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Controllers;

public class ReviewEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public ReviewEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        factory.ReviewSummaryQueryService.Reset();
    }

    [Fact]
    public async Task Get_ReviewInit_Returns200WithReviews()
    {
        _factory.ReviewSummaryQueryService
            .Setup(q => q.GetApprovedReviewsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReviewSummary>
            {
                new()
                {
                    PropertyId = 1,
                    PropertyName = "Lymm Holiday Let",
                    Name = "John Doe",
                    ReviewType = "Google",
                    Description = "Wonderful stay!"
                }
            });

        var response = await _client.GetAsync("/api/v1/review/init");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ReviewSummary>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Get_ReviewInit_WhenNoReviews_Returns200WithEmptyList()
    {
        _factory.ReviewSummaryQueryService
            .Setup(q => q.GetApprovedReviewsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReviewSummary>());

        var response = await _client.GetAsync("/api/v1/review/init");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
