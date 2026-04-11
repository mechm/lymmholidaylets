using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Controllers;

public class PageEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static PageDetail VisiblePage(string alias) =>
        new(alias, "Meta", "Title", "img.jpg", "Alt", "Content", "standard", visible: true);

    [Fact]
    public async Task Get_PageDetail_ValidAlias_Returns200()
    {
        factory.PageQueryService
            .Setup(q => q.GetVisiblePageByAliasAsync("about-us", It.IsAny<CancellationToken>()))
            .ReturnsAsync(VisiblePage("about-us"));

        var response = await _client.GetAsync("/api/v1/page/detail/about-us");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PageDetail>>();
        body!.Success.Should().BeTrue();
        body.Data!.AliasTitle.Should().Be("about-us");
    }

    [Fact]
    public async Task Get_PageDetail_PageNotFound_Returns404()
    {
        factory.PageQueryService
            .Setup(q => q.GetVisiblePageByAliasAsync("non-existent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((PageDetail?)null);

        var response = await _client.GetAsync("/api/v1/page/detail/non-existent");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_PageDetail_HiddenPage_Returns404()
    {
        factory.PageQueryService
            .Setup(q => q.GetVisiblePageByAliasAsync("hidden", It.IsAny<CancellationToken>()))
            .ReturnsAsync((PageDetail?)null);

        var response = await _client.GetAsync("/api/v1/page/detail/hidden");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
