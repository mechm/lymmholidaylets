using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Infrastructure.SiteMap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class SiteMapControllerTests
{
    private readonly Mock<ILogger<SiteMapController>> _logger = new();
    private readonly SiteMapController _sut;

    public SiteMapControllerTests()
    {
        _sut = new SiteMapController(_logger.Object);
    }

    [Fact]
    public void GetIndex_ReturnsXmlSiteMapIndex()
    {
        var result = _sut.GetIndex();

        result.Should().BeOfType<XmlSiteMapIndex>();
    }

    [Fact]
    public void GetIndex_LogsInformation()
    {
        _sut.GetIndex();

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Sitemap index requested")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
