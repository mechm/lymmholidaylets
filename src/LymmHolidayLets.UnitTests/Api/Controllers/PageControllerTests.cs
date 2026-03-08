using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.ReadModel.Page;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class PageControllerTests
{
    private readonly Mock<IPageQuery> _pageQuery = new();
    private readonly Mock<ILogger<PageController>> _logger = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private PageController CreateSut() =>
        new(_cache, _logger.Object, _pageQuery.Object);

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
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("missing-page")).ReturnsAsync((PageDetail?)null);

        var result = await CreateSut().Detail("missing-page");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Detail_PageNotVisible_ReturnsNotFound()
    {
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("hidden-page")).ReturnsAsync(HiddenPage());

        var result = await CreateSut().Detail("hidden-page");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Detail_ValidVisiblePage_ReturnsOk()
    {
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("about-us")).ReturnsAsync(VisiblePage());

        var result = await CreateSut().Detail("about-us");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<PageDetail>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.AliasTitle.Should().Be("about-us");
    }

    [Fact]
    public async Task Detail_CachesResultOnSecondCall()
    {
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("about-us")).ReturnsAsync(VisiblePage());
        var sut = CreateSut();

        await sut.Detail("about-us");
        await sut.Detail("about-us");

        _pageQuery.Verify(q => q.GetPageByAliasTitleAsync("about-us"), Times.Once);
    }
}
