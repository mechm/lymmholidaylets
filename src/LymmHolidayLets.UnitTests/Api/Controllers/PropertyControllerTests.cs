using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Property;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class PropertyControllerTests
{
    private readonly Mock<IPropertyQuery> _propertyQuery = new();
    private readonly Mock<ILogger<PropertyController>> _logger = new();
    private readonly Mock<IPropertyDetailResponseBuilder> _responseBuilder = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private PropertyController CreateSut() =>
        new(_cache, _logger.Object, _propertyQuery.Object, _responseBuilder.Object);

    private static PropertyDetailResult SomeDetail(byte id = 1) => new()
    {
        PropertyId           = id,
        DisplayAddress       = "Lymm Holiday Cottage",
        Description          = "A beautiful cottage in Lymm",
        MinimumNumberOfAdult = 1,
        MaximumNumberOfGuests = 6,
        MaximumNumberOfAdult  = 4,
        MaximumNumberOfChildren = 2,
        MaximumNumberOfInfants  = 2,
    };

    private static PropertyDetailResponse SomeResponse() => new()
    {
        PropertyId            = 1,
        DisplayAddress        = "Lymm Holiday Cottage",
        MinimumNumberOfAdult  = 1,
        MaximumNumberOfGuests = 6,
        MaximumNumberOfAdult  = 4,
        MaximumNumberOfChildren = 2,
        MaximumNumberOfInfants  = 2,
        ShareLinks = new PropertyShareLinksResponse
        {
            Facebook = "https://facebook.com/share",
            Twitter  = "https://twitter.com/share",
            LinkedIn = "https://linkedin.com/share",
            Email    = "mailto:?subject=..."
        },
        Seo = new PropertySeoResult
        {
            MetaTitle       = "Test Property | Lymm Holiday Lets",
            MetaDescription = "Test description",
            CanonicalUrl    = "https://example.com/property/1",
            OgTitle         = "Test Property | Lymm Holiday Lets",
            OgDescription   = "Test description"
        },
        SchemaOrg = new Dictionary<string, object> { ["@type"] = "LodgingBusiness" }
    };

    [Fact]
    public async Task Detail_PropertyNotFound_ReturnsNotFound()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync((PropertyDetailResult?)null);

        var result = await CreateSut().Detail(1);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Detail_PropertyFound_ReturnsOkWrappedInApiResponse()
    {
        var detail   = SomeDetail();
        var response = SomeResponse();

        _propertyQuery.Setup(q => q.GetPropertyDetailByIdAsync(1)).ReturnsAsync(detail);
        _responseBuilder.Setup(b => b.Build(detail)).Returns(response);

        var result = await CreateSut().Detail(1);

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<PropertyDetailResponse>>().Subject;
        body.Success.Should().BeTrue();
        body.Data.Should().BeSameAs(response);
    }

    [Fact]
    public async Task Detail_PropertyFound_DelegatesToResponseBuilder()
    {
        var detail = SomeDetail();
        _propertyQuery.Setup(q => q.GetPropertyDetailByIdAsync(1)).ReturnsAsync(detail);
        _responseBuilder.Setup(b => b.Build(It.IsAny<PropertyDetailResult>())).Returns(SomeResponse());

        await CreateSut().Detail(1);

        _responseBuilder.Verify(b => b.Build(detail), Times.Once);
    }

    [Fact]
    public async Task Detail_CachesResultOnSecondCall()
    {
        _propertyQuery.Setup(q => q.GetPropertyDetailByIdAsync(1)).ReturnsAsync(SomeDetail());
        _propertyQuery.Setup(q => q.GetCalendarLastModifiedAsync(1)).ReturnsAsync((DateTime?)null);
        _responseBuilder.Setup(b => b.Build(It.IsAny<PropertyDetailResult>())).Returns(SomeResponse());

        var sut = CreateSut();
        await sut.Detail(1);
        await sut.Detail(1);

        // DB must only be hit once — second call served from cache
        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Detail_DoesNotCacheWhenPropertyNotFound()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync((PropertyDetailResult?)null);

        var sut = CreateSut();
        await sut.Detail(1);
        await sut.Detail(1);

        // Both calls must hit DB — null results must never be cached
        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Exactly(2));
    }

    [Fact]
    public async Task Detail_EvictsCacheAndRefetchesWhenCalendarTimestampChanges()
    {
        var cached    = SomeDetail();          // CalendarLastModified = null
        var refreshed = SomeDetail();

        _propertyQuery.SetupSequence(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(cached)
            .ReturnsAsync(refreshed);

        // Second call: timestamp has changed (importer ran)
        _propertyQuery.Setup(q => q.GetCalendarLastModifiedAsync(1))
            .ReturnsAsync(new DateTime(2026, 4, 6, 10, 30, 0, DateTimeKind.Utc));

        _responseBuilder.Setup(b => b.Build(It.IsAny<PropertyDetailResult>())).Returns(SomeResponse());

        var sut = CreateSut();
        await sut.Detail(1);   // populates cache (CalendarLastModified = null)
        await sut.Detail(1);   // hit → timestamp mismatch → evict → re-fetch

        // DB hit twice: initial load + re-fetch after eviction
        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Exactly(2));
    }

    [Fact]
    public async Task Detail_ServesFromCacheWhenCalendarTimestampUnchanged()
    {
        var ts     = new DateTime(2026, 4, 6, 9, 0, 0, DateTimeKind.Utc);
        var detail = new PropertyDetailResult
        {
            PropertyId              = 1,
            MinimumNumberOfAdult    = 1,
            MaximumNumberOfGuests   = 4,
            MaximumNumberOfAdult    = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants  = 0,
            CalendarLastModified    = ts
        };

        _propertyQuery.Setup(q => q.GetPropertyDetailByIdAsync(1)).ReturnsAsync(detail);
        _propertyQuery.Setup(q => q.GetCalendarLastModifiedAsync(1)).ReturnsAsync(ts);
        _responseBuilder.Setup(b => b.Build(It.IsAny<PropertyDetailResult>())).Returns(SomeResponse());

        var sut = CreateSut();
        await sut.Detail(1);
        await sut.Detail(1);
        await sut.Detail(1);

        // DB only hit once — timestamps match on every cache hit
        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Once);
    }
}

