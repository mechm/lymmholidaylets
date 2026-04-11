using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Page;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class PageControllerTests
{
    private readonly Mock<IPageQueryService> _pageQueryService = new();
    private readonly Mock<ILogger<PageController>> _logger = new();

    private PageController CreateSut() =>
        new(_logger.Object, _pageQueryService.Object);

    private static PageDetail VisiblePage() =>
        new("about-us", "About our holiday let", "About Us",
            "about.jpg", "About image", "Description text", "standard", visible: true);

    private static PageDetail HiddenPage() =>
        new("hidden-page", "Hidden", "Hidden Page",
            "img.jpg", "Alt", "Content", "standard", visible: false);

    [Fact]
    public async Task Detail_EmptyId_ReturnsBadRequest()
    {
        var result = await CreateSut().Detail("");

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Detail_WhitespaceId_ReturnsBadRequest()
    {
        var result = await CreateSut().Detail("   ");

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Detail_PageNotFound_ReturnsNotFound()
    {
        _pageQueryService.Setup(q => q.GetVisiblePageByAliasAsync("missing-page", It.IsAny<CancellationToken>())).ReturnsAsync((PageDetail?)null);

        var result = await CreateSut().Detail("missing-page");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Detail_PageNotVisible_ReturnsNotFound()
    {
        _pageQueryService.Setup(q => q.GetVisiblePageByAliasAsync("hidden-page", It.IsAny<CancellationToken>())).ReturnsAsync((PageDetail?)null);

        var result = await CreateSut().Detail("hidden-page");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Detail_ValidVisiblePage_ReturnsOk()
    {
        _pageQueryService.Setup(q => q.GetVisiblePageByAliasAsync("about-us", It.IsAny<CancellationToken>())).ReturnsAsync(VisiblePage());

        var result = await CreateSut().Detail("about-us");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<PageDetail>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.AliasTitle.Should().Be("about-us");
    }

}
