using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Controllers;

public class HomepageEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_HomepageInit_Returns200WithSuccessBody()
    {
        factory.HomepageQueryService
            .Setup(s => s.GetHomepageDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HomepageResult([], []));

        var response = await _client.GetAsync("/api/v1/homepage/init");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<HomepageModel>>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Get_HomepageInit_WhenServiceReturnsNull_Returns500()
    {
        factory.HomepageQueryService
            .Setup(s => s.GetHomepageDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((HomepageResult?)null);

        var response = await _client.GetAsync("/api/v1/homepage/init");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
