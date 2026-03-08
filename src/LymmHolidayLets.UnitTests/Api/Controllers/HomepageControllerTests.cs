using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class HomepageControllerTests
{
    private readonly Mock<IHomepageService> _homepageService = new();
    private readonly Mock<ILogger<HomepageController>> _logger = new();
    private readonly HomepageController _sut;

    public HomepageControllerTests()
    {
        _sut = new HomepageController(_homepageService.Object, _logger.Object);
    }

    [Fact]
    public async Task Index_WhenServiceReturnsNull_Returns500()
    {
        _homepageService.Setup(s => s.GetHomepageDataAsync()).ReturnsAsync((HomepageModel?)null);

        var result = await _sut.Index();

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Index_WhenSuccess_ReturnsOkWithModel()
    {
        var model = new HomepageModel([], []);
        _homepageService.Setup(s => s.GetHomepageDataAsync()).ReturnsAsync(model);

        var result = await _sut.Index();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<HomepageModel>>().Subject;
        body.Success.Should().BeTrue();
        body.Data.Should().BeSameAs(model);
    }
}
