using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class HomepageControllerTests
{
    private readonly Mock<IHomepageQueryService> _homepageQueryService = new();
    private readonly Mock<ILogger<HomepageController>> _logger = new();
    private readonly HomepageController _sut;

    public HomepageControllerTests()
    {
        _sut = new HomepageController(_homepageQueryService.Object, _logger.Object);
    }

    [Fact]
    public async Task Get_WhenServiceReturnsNull_Returns500()
    {
        _homepageQueryService.Setup(s => s.GetHomepageDataAsync(It.IsAny<CancellationToken>())).ReturnsAsync((HomepageResult?)null);

        var result = await _sut.Get();

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Get_WhenSuccess_ReturnsOkWithModel()
    {
        var result = new HomepageResult([], []);
        _homepageQueryService.Setup(s => s.GetHomepageDataAsync(It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _sut.Get();

        var ok = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<HomepageModel>>().Subject;
        body.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }
}
