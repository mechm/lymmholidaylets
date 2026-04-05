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
    private readonly Mock<ISocialShareLinkGenerator> _socialShareLinkGenerator = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private PropertyController CreateSut() =>
        new(_cache, _logger.Object, _propertyQuery.Object, _socialShareLinkGenerator.Object);

    private static PropertyDetailResult PropertyDetail(byte id = 1) => new()
    {
        PropertyId              = id,
        DisplayAddress          = "Lymm Holiday Cottage",
        PageDescription         = "A beautiful cottage in Lymm",
        MinimumNumberOfAdult    = 1,
        MaximumNumberOfGuests   = 6,
        MaximumNumberOfAdult    = 4,
        MaximumNumberOfChildren = 2,
        MaximumNumberOfInfants  = 2,
        DatesBooked = [new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 2)],
        FaQs =
        [
            new PropertyFaqResult { Question = "Is parking available?", Answer = "Yes, free on-site parking." }
        ],
        ReviewAggregate = new PropertyReviewAggregateResult
        {
            OverallRating = 4.8,
            Reviews =
            [
                new PropertyReviewResult
                {
                    Name        = "Jane Smith",
                    Description = "Wonderful stay.",
                    Rating      = 5,
                    ReviewType  = "Airbnb"
                }
            ]
        },
        Host = new PropertyHostResult
        {
            Name               = "John Doe",
            Location           = "Lymm, UK",
            NumberOfProperties = 3,
            YearsExperience    = 5,
            JobTitle           = "Property Manager",
            ProfileBio         = "Experienced host with a passion for hospitality.",
            ImagePath          = "/images/host.jpg"
        },
        Map = new PropertyMapResult
        {
            ShowMap              = true,
            ShowStreetView       = true,
            Latitude             = 53.3811,
            Longitude            = -2.4730,
            MapZoom              = 15,
            StreetViewLatitude   = 53.3812,
            StreetViewLongitude  = -2.4731,
            Pitch                = 10,
            Yaw                  = 165,
            Zoom                 = 1
        }
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
    public async Task Detail_PropertyFound_ReturnsOkWithDetail()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(PropertyDetail());
        
        _socialShareLinkGenerator
            .Setup(g => g.GenerateLinks(1, "Lymm Holiday Cottage"))
            .Returns(new SocialShareLinks
            {
                PropertyUrl = "https://example.com/property/1",
                FacebookShareLink = "https://facebook.com/sharer/sharer.php?u=...",
                TwitterShareLink = "https://twitter.com/share?text=...",
                LinkedInShareLink = "https://linkedin.com/shareArticle?...",
                EmailShareLink = "mailto:?subject=..."
            });

        var result = await CreateSut().Detail(1);

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<PropertyDetailResponse>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.PropertyDetail.MaximumNumberOfGuests.Should().Be(6);
        body.Data.PropertyDetail.FaQs.Should().HaveCount(1);
        body.Data.PropertyDetail.ReviewAggregate.Should().NotBeNull();
        body.Data.PropertyDetail.ReviewAggregate!.OverallRating.Should().Be(4.8);
        body.Data.PropertyUrl.Should().Be("https://example.com/property/1");
    }

    [Fact]
    public async Task Detail_PropertyWithNoReviews_ReturnsOkWithNullAggregate()
    {
        var noReviews = new PropertyDetailResult
        {
            PropertyId              = 1,
            DisplayAddress          = "Test Address",
            PageDescription         = "Test Description",
            MinimumNumberOfAdult    = 1,
            MaximumNumberOfGuests   = 6,
            MaximumNumberOfAdult    = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants  = 2,
            DatesBooked     = [],
            FaQs            = [],
            ReviewAggregate = null
        };
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(noReviews);
        
        _socialShareLinkGenerator
            .Setup(g => g.GenerateLinks(It.IsAny<byte>(), It.IsAny<string>()))
            .Returns(new SocialShareLinks
            {
                PropertyUrl = "https://example.com/property/1",
                FacebookShareLink = "",
                TwitterShareLink = "",
                LinkedInShareLink = "",
                EmailShareLink = ""
            });

        var result = await CreateSut().Detail(1);

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<PropertyDetailResponse>>().Subject;
        body.Data!.PropertyDetail.ReviewAggregate.Should().BeNull();
    }

    [Fact]
    public async Task Detail_PropertyFound_ReturnsHostAndMapInformation()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(PropertyDetail());
        
        _socialShareLinkGenerator
            .Setup(g => g.GenerateLinks(1, "Lymm Holiday Cottage"))
            .Returns(new SocialShareLinks
            {
                PropertyUrl = "https://example.com/property/1",
                FacebookShareLink = "https://facebook.com/sharer/sharer.php?u=...",
                TwitterShareLink = "https://twitter.com/share?text=...",
                LinkedInShareLink = "https://linkedin.com/shareArticle?...",
                EmailShareLink = "mailto:?subject=..."
            });

        var result = await CreateSut().Detail(1);

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<PropertyDetailResponse>>().Subject;
        
        // Verify host information
        body.Data!.PropertyDetail.Host.Should().NotBeNull();
        body.Data.PropertyDetail.Host!.Name.Should().Be("John Doe");
        body.Data.PropertyDetail.Host.YearsExperience.Should().Be(5);
        body.Data.PropertyDetail.Host.NumberOfProperties.Should().Be(3);
        
        // Verify map information
        body.Data.PropertyDetail.Map.Should().NotBeNull();
        body.Data.PropertyDetail.Map!.ShowMap.Should().BeTrue();
        body.Data.PropertyDetail.Map.Latitude.Should().Be(53.3811);
        body.Data.PropertyDetail.Map.Longitude.Should().Be(-2.4730);
        
        // Verify basic property info
        body.Data.PropertyDetail.PropertyId.Should().Be(1);
        body.Data.PropertyDetail.DisplayAddress.Should().Be("Lymm Holiday Cottage");
        body.Data.PropertyDetail.PageDescription.Should().Be("A beautiful cottage in Lymm");
        
        // Verify social sharing links
        body.Data.PropertyUrl.Should().Be("https://example.com/property/1");
        body.Data.FacebookShareLink.Should().NotBeEmpty();
        body.Data.TwitterShareLink.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Detail_CachesResultOnSecondCall()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(PropertyDetail());
        
        _socialShareLinkGenerator
            .Setup(g => g.GenerateLinks(It.IsAny<byte>(), It.IsAny<string>()))
            .Returns(new SocialShareLinks
            {
                PropertyUrl = "https://example.com/property/1",
                FacebookShareLink = "",
                TwitterShareLink = "",
                LinkedInShareLink = "",
                EmailShareLink = ""
            });
        
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
        
        _socialShareLinkGenerator
            .Setup(g => g.GenerateLinks(It.IsAny<byte>(), It.IsAny<string>()))
            .Returns(new SocialShareLinks
            {
                PropertyUrl = "",
                FacebookShareLink = "",
                TwitterShareLink = "",
                LinkedInShareLink = "",
                EmailShareLink = ""
            });
        
        var sut = CreateSut();

        await sut.Detail(1);
        await sut.Detail(1);

        // Both calls must hit the DB — null results must never be cached
        _propertyQuery.Verify(q => q.GetPropertyDetailByIdAsync(1), Times.Exactly(2));
    }
}

