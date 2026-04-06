using FluentAssertions;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Model.Property;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Services;

public class PropertyDetailResponseBuilderTests
{
    private readonly Mock<ISocialShareLinkGenerator> _shareLinks = new();
    private readonly Mock<ISeoMetaGenerator> _seoMeta = new();
    private readonly Mock<ISchemaOrgGenerator> _schemaOrg = new();
    private readonly Mock<IImageUrlResolver> _imageUrlResolver = new();

    private PropertyDetailResponseBuilder CreateSut() =>
        new(_shareLinks.Object, _seoMeta.Object, _schemaOrg.Object, _imageUrlResolver.Object);

    public PropertyDetailResponseBuilderTests()
    {
        _shareLinks
            .Setup(s => s.GenerateLinks(It.IsAny<byte>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(new SocialShareLinks
            {
                PropertyUrl       = "https://www.lymmholidaylets.co.uk/property/1",
                FacebookShareLink = "https://facebook.com/share",
                TwitterShareLink  = "https://twitter.com/share",
                LinkedInShareLink = "https://linkedin.com/share",
                EmailShareLink    = "mailto:?subject=..."
            });

        _seoMeta
            .Setup(s => s.Generate(It.IsAny<PropertyDetailResult>(), It.IsAny<string>()))
            .Returns(new PropertySeoResult
            {
                MetaTitle       = "Test | Lymm Holiday Lets",
                MetaDescription = "Test description",
                CanonicalUrl    = "https://www.lymmholidaylets.co.uk/property/1",
                OgTitle         = "Test | Lymm Holiday Lets",
                OgDescription   = "Test description"
            });

        _schemaOrg
            .Setup(s => s.Generate(It.IsAny<PropertyDetailResult>(), It.IsAny<string>()))
            .Returns(new Dictionary<string, object> { ["@type"] = "LodgingBusiness" });

        _imageUrlResolver
            .Setup(r => r.Resolve(It.IsAny<string?>()))
            .Returns<string?>(p => string.IsNullOrWhiteSpace(p) ? null
                : p.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? p
                : $"https://www.lymmholidaylets.co.uk{p}");
    }

    private static PropertyDetailResult MinimalDetail(byte id = 1) => new()
    {
        PropertyId            = id,
        DisplayAddress        = "The Granary, Lymm",
        MinimumNumberOfAdult  = 2,
        MaximumNumberOfGuests = 8,
        MaximumNumberOfAdult  = 6,
        MaximumNumberOfChildren = 4,
        MaximumNumberOfInfants  = 2,
    };

    [Fact]
    public void Build_MapsBasicPropertyFields()
    {
        var detail = MinimalDetail();

        var response = CreateSut().Build(detail);

        response.PropertyId.Should().Be(1);
        response.DisplayAddress.Should().Be("The Granary, Lymm");
        response.MaximumNumberOfGuests.Should().Be(8);
    }

    [Fact]
    public void Build_NullHost_HostIsNull()
    {
        var detail = MinimalDetail();

        var response = CreateSut().Build(detail);

        response.Host.Should().BeNull();
    }

    [Fact]
    public void Build_MapsHostFields_IncludingLocation()
    {
        var detail = new PropertyDetailResult
        {
            PropertyId              = 1,
            MinimumNumberOfAdult    = 2,
            MaximumNumberOfGuests   = 8,
            MaximumNumberOfAdult    = 6,
            MaximumNumberOfChildren = 4,
            MaximumNumberOfInfants  = 2,
            Host = new PropertyHostResult
            {
                Name               = "Matt Chambers",
                JobTitle           = "Property Manager",
                NumberOfProperties = 2,
                YearsExperience    = 8,
                ProfileBio         = "Passionate host.",
                Location           = "Lymm, Cheshire",
                ImagePath          = "/images/staff/matt.jpg"
            }
        };

        var response = CreateSut().Build(detail);

        response.Host.Should().NotBeNull();
        response.Host!.Name.Should().Be("Matt Chambers");
        response.Host.JobTitle.Should().Be("Property Manager");
        response.Host.NumberOfProperties.Should().Be(2);
        response.Host.YearsExperience.Should().Be(8);
        response.Host.ProfileBio.Should().Be("Passionate host.");
        response.Host.Location.Should().Be("Lymm, Cheshire");
    }

    [Fact]
    public void Build_ResolvesHostImageToAbsoluteUrl()
    {
        var detail = new PropertyDetailResult
        {
            PropertyId            = 1,
            MinimumNumberOfAdult  = 1,
            MaximumNumberOfGuests = 4,
            MaximumNumberOfAdult  = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants  = 0,
            Host = new PropertyHostResult
            {
                Name               = "Host",
                JobTitle           = "Host",
                NumberOfProperties = 1,
                YearsExperience    = 1,
                ImagePath          = "/images/staff/host.jpg"
            }
        };

        var response = CreateSut().Build(detail);

        response.Host!.ImagePath.Should().Be("https://www.lymmholidaylets.co.uk/images/staff/host.jpg");
    }

    [Fact]
    public void Build_ResolvesPropertyImagesToAbsoluteUrl()
    {
        var detail = new PropertyDetailResult
        {
            PropertyId              = 1,
            MinimumNumberOfAdult    = 2,
            MaximumNumberOfGuests   = 8,
            MaximumNumberOfAdult    = 6,
            MaximumNumberOfChildren = 4,
            MaximumNumberOfInfants  = 2,
            Images =
            [
                new PropertyImageResult { ImagePath = "/images/prop/exterior.jpg", AltText = "Exterior", SequenceOrder = 1 },
                new PropertyImageResult { ImagePath = "https://cdn.example.com/img.jpg", AltText = "CDN image", SequenceOrder = 2 }
            ]
        };

        var response = CreateSut().Build(detail);

        response.Images[0].ImagePath.Should().Be("https://www.lymmholidaylets.co.uk/images/prop/exterior.jpg");
        response.Images[1].ImagePath.Should().Be("https://cdn.example.com/img.jpg"); // already absolute — unchanged
    }

    [Fact]
    public void Build_NullRatingSummary_RatingSummaryIsNull()
    {
        var response = CreateSut().Build(MinimalDetail());

        response.RatingSummary.Should().BeNull();
    }

    [Fact]
    public void Build_RatingSummary_RoundsRatingToTwoDecimalPlaces()
    {
        var detail = new PropertyDetailResult
        {
            PropertyId            = 1,
            MinimumNumberOfAdult  = 1,
            MaximumNumberOfGuests = 4,
            MaximumNumberOfAdult  = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants  = 0,
            RatingSummary = new PropertyRatingSummaryResult { Rating = 4.8666, TotalReviews = 15 }
        };

        var response = CreateSut().Build(detail);

        response.RatingSummary!.Rating.Should().Be(4.87);
        response.RatingSummary.TotalReviews.Should().Be(15);
    }

    [Fact]
    public void Build_MapsShareLinks()
    {
        var response = CreateSut().Build(MinimalDetail());

        response.ShareLinks.Facebook.Should().Be("https://facebook.com/share");
        response.ShareLinks.Twitter.Should().Be("https://twitter.com/share");
        response.ShareLinks.LinkedIn.Should().Be("https://linkedin.com/share");
        response.ShareLinks.Email.Should().Be("mailto:?subject=...");
    }

    [Fact]
    public void Build_MapsSeoFromGenerator()
    {
        var response = CreateSut().Build(MinimalDetail());

        response.Seo.CanonicalUrl.Should().Be("https://www.lymmholidaylets.co.uk/property/1");
        response.Seo.MetaTitle.Should().Be("Test | Lymm Holiday Lets");
    }

    [Fact]
    public void Build_MapsReviews_WithDateToDisplay()
    {
        var added = DateTime.UtcNow.AddDays(-3);
        var detail = new PropertyDetailResult
        {
            PropertyId            = 1,
            MinimumNumberOfAdult  = 1,
            MaximumNumberOfGuests = 4,
            MaximumNumberOfAdult  = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants  = 0,
            Reviews =
            [
                new PropertyReviewResult
                {
                    Name          = "Jane Smith",
                    Description   = "Fantastic stay!",
                    Rating        = 5,
                    ReviewType    = "Google",
                    DateTimeAdded = added
                }
            ]
        };

        var response = CreateSut().Build(detail);

        response.Reviews.Should().HaveCount(1);
        response.Reviews[0].Name.Should().Be("Jane Smith");
        response.Reviews[0].DateToDisplay.Should().Be("3 days ago");
    }
}
