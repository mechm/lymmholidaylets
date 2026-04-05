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
    private readonly Mock<ISeoMetaGenerator> _seoMetaGenerator = new();
    private readonly Mock<ISchemaOrgGenerator> _schemaOrgGenerator = new();
    private readonly Mock<IImageUrlResolver> _imageUrlResolver = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private PropertyController CreateSut() =>
        new(_cache, _logger.Object, _propertyQuery.Object, _socialShareLinkGenerator.Object,
            _seoMetaGenerator.Object, _schemaOrgGenerator.Object, _imageUrlResolver.Object);

    public PropertyControllerTests()
    {
        _seoMetaGenerator
            .Setup(s => s.Generate(It.IsAny<PropertyDetailResult>(), It.IsAny<string>()))
            .Returns(new PropertySeoResult
            {
                MetaTitle       = "Test Property | Lymm Holiday Lets",
                MetaDescription = "Test description",
                CanonicalUrl    = "https://example.com/property/1",
                OgTitle         = "Test Property | Lymm Holiday Lets",
                OgDescription   = "Test description"
            });

        _schemaOrgGenerator
            .Setup(s => s.Generate(It.IsAny<PropertyDetailResult>(), It.IsAny<string>()))
            .Returns(new Dictionary<string, object> { ["@type"] = "LodgingBusiness" });

        // Return absolute URL for any path; pass through already-absolute paths unchanged
        _imageUrlResolver
            .Setup(r => r.Resolve(It.IsAny<string?>()))
            .Returns<string?>(p => string.IsNullOrWhiteSpace(p) ? null
                : p.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? p
                : $"https://www.lymmholidaylets.co.uk{p}");
    }

    private static PropertyDetailResult PropertyDetail(byte id = 1) => new()
    {
        PropertyId              = id,
        DisplayAddress          = "Lymm Holiday Cottage",
        Description         = "A beautiful cottage in Lymm",
        MinimumNumberOfAdult    = 1,
        MaximumNumberOfGuests   = 6,
        MaximumNumberOfAdult    = 4,
        MaximumNumberOfChildren = 2,
        MaximumNumberOfInfants  = 2,
        DatesBooked = [new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 2)],
        Faqs =
        [
            new PropertyFaqResult { Question = "Is parking available?", Answer = "Yes, free on-site parking." }
        ],
        RatingSummary = new PropertyRatingSummaryResult
        {
            Rating = 4.8
        },
        Reviews =
        [
            new PropertyReviewResult
            {
                Name        = "Jane Smith",
                Description = "Wonderful stay.",
                Rating      = 5,
                ReviewType  = "Airbnb"
            }
        ],
        Host = new PropertyHostResult
        {
            Name               = "John Doe",
            NumberOfProperties = 3,
            YearsExperience    = 5,
            JobTitle           = "Property Manager",
            ProfileBio         = "Experienced host with a passion for hospitality.",
            ImagePath          = "/images/host.jpg"
        },
        Map = new PropertyMapResult
        {
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
            .Setup(g => g.GenerateLinks(1, "Lymm Holiday Cottage", null))
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
        body.Data!.MaximumNumberOfGuests.Should().Be(6);
        body.Data.Faqs.Should().HaveCount(1);
        body.Data.RatingSummary.Should().NotBeNull();
        body.Data.RatingSummary!.Rating.Should().Be(4.8);
        body.Data.Seo.CanonicalUrl.Should().Be("https://example.com/property/1");
    }

    [Fact]
    public async Task Detail_PropertyWithNoReviews_ReturnsOkWithNullAggregate()
    {
        var noReviews = new PropertyDetailResult
        {
            PropertyId              = 1,
            DisplayAddress          = "Test Address",
            Description         = "Test Description",
            MinimumNumberOfAdult    = 1,
            MaximumNumberOfGuests   = 6,
            MaximumNumberOfAdult    = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants  = 2,
            DatesBooked     = [],
            Faqs            = [],
            RatingSummary   = null
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
        body.Data!.RatingSummary.Should().BeNull();
    }

    [Fact]
    public async Task Detail_PropertyFound_ReturnsHostAndMapInformation()
    {
        _propertyQuery
            .Setup(q => q.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(PropertyDetail());
        
        _socialShareLinkGenerator
            .Setup(g => g.GenerateLinks(1, "Lymm Holiday Cottage", null))
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
        body.Data!.Host.Should().NotBeNull();
        body.Data.Host!.Name.Should().Be("John Doe");
        body.Data.Host.YearsExperience.Should().Be(5);
        body.Data.Host.NumberOfProperties.Should().Be(3);
        
        // Verify map information
        body.Data.Map.Should().NotBeNull();
        body.Data.Map!.Latitude.Should().Be(53.3811);
        body.Data.Map.Longitude.Should().Be(-2.4730);
        
        // Verify basic property info
        body.Data.PropertyId.Should().Be(1);
        body.Data.DisplayAddress.Should().Be("Lymm Holiday Cottage");
        body.Data.Description.Should().Be("A beautiful cottage in Lymm");
        
        // Verify social sharing links
        body.Data.Seo.CanonicalUrl.Should().Be("https://example.com/property/1");
        body.Data.ShareLinks.Facebook.Should().NotBeEmpty();
        body.Data.ShareLinks.Twitter.Should().NotBeEmpty();
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

