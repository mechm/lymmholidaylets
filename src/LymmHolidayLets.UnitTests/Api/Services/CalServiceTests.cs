using System.Text;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Services;

public class CalServiceTests
{
    private readonly Mock<ICalQuery> _icalQuery = new();
    private readonly Mock<ICalGenerator> _icalGenerator = new();
    private readonly Mock<ILogger<CalService>> _logger = new();

    private CalService CreateService(IMemoryCache cache)
        => new CalService(cache, _icalQuery.Object, _icalGenerator.Object, _logger.Object);

    private static MemoryCache CreateCache(IReadOnlyList<ICal>? icalList = null)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        if (icalList != null)
            cache.Set("ical-results", icalList);
        return cache;
    }

    private static ICal CreateICal(byte propertyId, Guid identifier)
        => new ICal(1, propertyId, "Test Property", identifier);

    [Fact]
    public async Task GetCalendarAsync_PropertyIdZero_ReturnsNull()
    {
        using var cache = CreateCache();
        var service = CreateService(cache);
        var result = await service.GetCalendarAsync(0, Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCalendarAsync_NoMatchingCalendar_ReturnsNull()
    {
        using var cache = CreateCache(new List<ICal>());
        _icalQuery.Setup(q => q.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ICal>());
        var service = CreateService(cache);

        var result = await service.GetCalendarAsync(1, Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCalendarAsync_GenerationFails_ReturnsNull()
    {
        byte id = 1;
        var guid = Guid.NewGuid();
        var icalList = new List<ICal> { CreateICal(id, guid) };
        using var cache = CreateCache(icalList);

        _icalGenerator.Setup(g => g.GenerateCalendarAsync(id)).ThrowsAsync(new Exception("fail"));
        var service = CreateService(cache);

        var result = await service.GetCalendarAsync(id, guid);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCalendarAsync_EmptyCalendar_ReturnsNull()
    {
        byte id = 1;
        var guid = Guid.NewGuid();
        var icalList = new List<ICal> { CreateICal(id, guid) };
        using var cache = CreateCache(icalList);

        _icalGenerator.Setup(g => g.GenerateCalendarAsync(id)).ReturnsAsync("");
        var service = CreateService(cache);

        var result = await service.GetCalendarAsync(id, guid);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCalendarAsync_Valid_ReturnsFileContentResultWithUtf8()
    {
        byte id = 1;
        var guid = Guid.NewGuid();
        var calendarString = "BEGIN:VCALENDAR";
        var icalList = new List<ICal> { CreateICal(id, guid) };
        using var cache = CreateCache(icalList);

        _icalGenerator.Setup(g => g.GenerateCalendarAsync(id)).ReturnsAsync(calendarString);
        var service = CreateService(cache);

        var result = await service.GetCalendarAsync(id, guid);
        Assert.NotNull(result);
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/calendar; charset=utf-8", result.ContentType);
        Assert.Equal($"{id}.ics", result.FileDownloadName);
        Assert.Equal(Encoding.UTF8.GetBytes(calendarString), result.FileContents);
    }

    [Fact]
    public async Task GetCalendarAsync_CachedCalendar_DoesNotCallGenerator()
    {
        byte id = 1;
        var guid = Guid.NewGuid();
        var calendarString = "BEGIN:VCALENDAR";
        var icalList = new List<ICal> { CreateICal(id, guid) };
        using var cache = CreateCache(icalList);

        cache.Set($"ical-availability-{id}", calendarString);

        var service = CreateService(cache);
        var result = await service.GetCalendarAsync(id, guid);

        Assert.NotNull(result);
        Assert.Equal("text/calendar; charset=utf-8", result.ContentType);
        _icalGenerator.Verify(g => g.GenerateCalendarAsync(It.IsAny<byte>()), Times.Never);
    }
}
