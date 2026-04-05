using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Domain.ReadModel.Review;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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

        // Clear the in-memory cache and reset the mock before every test so
        // cached data from a prior test never leaks into the next one.
        var cache = factory.Services.GetRequiredService<IMemoryCache>();
        cache.Remove("reviews");
        factory.ReviewQuery.Reset();
    }

    [Fact]
    public async Task Get_ReviewInit_Returns200WithReviews()
    {
        _factory.ReviewQuery
            .Setup(q => q.GetAllApprovedReviewsAsync())
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
        _factory.ReviewQuery
            .Setup(q => q.GetAllApprovedReviewsAsync())
            .ReturnsAsync(new List<ReviewSummary>());

        var response = await _client.GetAsync("/api/v1/review/init");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
