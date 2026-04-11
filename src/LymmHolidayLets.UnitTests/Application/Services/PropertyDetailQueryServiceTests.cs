using FluentAssertions;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Property;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class PropertyDetailQueryServiceTests
{
    private readonly Mock<IPropertyQuery> _propertyQuery = new();
    private readonly Mock<ILogger<PropertyDetailQueryService>> _logger = new();
    private readonly ApplicationMemoryCache _cache = new(new MemoryCache(new MemoryCacheOptions()));

    private PropertyDetailQueryService CreateSut() => new(_cache, _propertyQuery.Object, _logger.Object);

    private static PropertyDetailResult SomeDetail(byte id = 1) => new()
    {
        PropertyId = id,
        DisplayAddress = "Lymm Holiday Cottage",
        Description = "A beautiful cottage in Lymm",
        MinimumNumberOfAdult = 1,
        MaximumNumberOfGuests = 6,
        MaximumNumberOfAdult = 4,
        MaximumNumberOfChildren = 2,
        MaximumNumberOfInfants = 2,
    };

    [Fact]
    public async Task GetPropertyDetailAsync_CachesResultOnSecondCall()
    {
        _propertyQuery.Setup(q => q.GetPropertyDetailByIdAsync(1)).ReturnsAsync(SomeDetail());
        _propertyQuery.Setup(q => q.GetCalendarLastModifiedAsync(1)).ReturnsAsync((DateTime?)null);
        var sut = CreateSut();

        await sut.GetPropertyDetailAsync(1);
        await sut.GetPropertyDetailAsync(1);

        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetPropertyDetailAsync_DoesNotCacheWhenPropertyNotFound()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync((PropertyDetailResult?)null);

        var sut = CreateSut();
        await sut.GetPropertyDetailAsync(1);
        await sut.GetPropertyDetailAsync(1);

        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Exactly(2));
    }

    [Fact]
    public async Task GetPropertyDetailAsync_EvictsCacheAndRefetchesWhenCalendarTimestampChanges()
    {
        var cached = SomeDetail();
        var refreshed = SomeDetail();

        _propertyQuery.SetupSequence(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(cached)
            .ReturnsAsync(refreshed);
        _propertyQuery.Setup(q => q.GetCalendarLastModifiedAsync(1))
            .ReturnsAsync(new DateTime(2026, 4, 6, 10, 30, 0, DateTimeKind.Utc));

        var sut = CreateSut();
        await sut.GetPropertyDetailAsync(1);
        await sut.GetPropertyDetailAsync(1);

        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Exactly(2));
    }

    [Fact]
    public async Task GetPropertyDetailAsync_ServesFromCacheWhenCalendarTimestampUnchanged()
    {
        var ts = new DateTime(2026, 4, 6, 9, 0, 0, DateTimeKind.Utc);
        var detail = new PropertyDetailResult
        {
            PropertyId = 1,
            MinimumNumberOfAdult = 1,
            MaximumNumberOfGuests = 4,
            MaximumNumberOfAdult = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants = 0,
            CalendarLastModified = ts
        };

        _propertyQuery.Setup(q => q.GetPropertyDetailByIdAsync(1)).ReturnsAsync(detail);
        _propertyQuery.Setup(q => q.GetCalendarLastModifiedAsync(1)).ReturnsAsync(ts);

        var sut = CreateSut();
        await sut.GetPropertyDetailAsync(1);
        await sut.GetPropertyDetailAsync(1);
        await sut.GetPropertyDetailAsync(1);

        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Once);
    }
}
