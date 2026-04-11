using FluentAssertions;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class PageQueryServiceTests
{
    private readonly Mock<IPageQuery> _pageQuery = new();
    private readonly Mock<ILogger<PageQueryService>> _logger = new();
    private readonly ApplicationMemoryCache _cache = new(new MemoryCache(new MemoryCacheOptions()));

    private PageQueryService CreateSut() => new(_cache, _pageQuery.Object, _logger.Object);

    private static PageDetail VisiblePage() =>
        new("about-us", "About our holiday let", "About Us",
            "about.jpg", "About image", "Description text", "standard", visible: true);

    private static PageDetail HiddenPage() =>
        new("hidden-page", "Hidden", "Hidden Page",
            "img.jpg", "Alt", "Content", "standard", visible: false);

    [Fact]
    public async Task GetVisiblePageByAliasAsync_PageNotFound_ReturnsNull()
    {
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("missing-page")).ReturnsAsync((PageDetail?)null);

        var result = await CreateSut().GetVisiblePageByAliasAsync("missing-page");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVisiblePageByAliasAsync_PageNotVisible_ReturnsNull()
    {
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("hidden-page")).ReturnsAsync(HiddenPage());

        var result = await CreateSut().GetVisiblePageByAliasAsync("hidden-page");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVisiblePageByAliasAsync_CachesVisibleResult()
    {
        _pageQuery.Setup(q => q.GetPageByAliasTitleAsync("about-us")).ReturnsAsync(VisiblePage());
        var sut = CreateSut();

        await sut.GetVisiblePageByAliasAsync("about-us");
        await sut.GetVisiblePageByAliasAsync("about-us");

        _pageQuery.Verify(q => q.GetPageByAliasTitleAsync("about-us"), Times.Once);
    }
}
