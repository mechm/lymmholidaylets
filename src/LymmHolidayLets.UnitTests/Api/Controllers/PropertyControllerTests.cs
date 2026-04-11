using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Property;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class PropertyControllerTests
{
    private readonly Mock<IPropertyDetailQueryService> _propertyDetailQueryService = new();
    private readonly Mock<ILogger<PropertyController>> _logger = new();
    private readonly Mock<IPropertyDetailResponseBuilder> _responseBuilder = new();

    private PropertyController CreateSut() =>
        new(_logger.Object, _propertyDetailQueryService.Object, _responseBuilder.Object);

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
        _propertyDetailQueryService
            .Setup(q => q.GetPropertyDetailAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyDetailResult?)null);

        var result = await CreateSut().Detail(1);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Detail_PropertyFound_ReturnsOkWrappedInApiResponse()
    {
        var detail   = SomeDetail();
        var response = SomeResponse();

        _propertyDetailQueryService.Setup(q => q.GetPropertyDetailAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(detail);
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
        _propertyDetailQueryService.Setup(q => q.GetPropertyDetailAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(detail);
        _responseBuilder.Setup(b => b.Build(It.IsAny<PropertyDetailResult>())).Returns(SomeResponse());

        await CreateSut().Detail(1);

        _responseBuilder.Verify(b => b.Build(detail), Times.Once);
    }

}

