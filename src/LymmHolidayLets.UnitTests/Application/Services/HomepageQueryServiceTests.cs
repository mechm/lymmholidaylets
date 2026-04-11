using FluentAssertions;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.ReadModel.Homepage;
using LymmHolidayLets.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class HomepageQueryServiceTests
{
    private readonly Mock<IHomepageQuery> _homepageQuery = new();
    private readonly Mock<ILogger<HomepageQueryService>> _logger = new();
    private readonly ApplicationMemoryCache _cache = new(new MemoryCache(new MemoryCacheOptions()));

    private HomepageQueryService CreateSut() => new(_cache, _homepageQuery.Object, _logger.Object);

    [Fact]
    public async Task GetHomepageDataAsync_CachesResultOnSecondCall()
    {
        _homepageQuery.Setup(q => q.GetHomePageDetailAsync())
            .ReturnsAsync(new HomepageAggregate([], []));

        var sut = CreateSut();
        await sut.GetHomepageDataAsync();
        await sut.GetHomepageDataAsync();

        _homepageQuery.Verify(q => q.GetHomePageDetailAsync(), Times.Once);
    }

    [Fact]
    public async Task GetHomepageDataAsync_WhenQueryThrows_ReturnsNull()
    {
        _homepageQuery.Setup(q => q.GetHomePageDetailAsync()).ThrowsAsync(new Exception("fail"));

        var result = await CreateSut().GetHomepageDataAsync();

        result.Should().BeNull();
    }
}
