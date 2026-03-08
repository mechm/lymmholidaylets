using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class CalendarControllerTests
{
    private readonly Mock<ICalService> _calService = new();
    private readonly Mock<ILogger<CalendarController>> _logger = new();

    private CalendarController CreateSut() => new(_calService.Object, _logger.Object);

    [Fact]
    public async Task ICal_WhenServiceReturnsNull_ReturnsBadRequest()
    {
        _calService.Setup(s => s.GetCalendarAsync(It.IsAny<byte>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FileContentResult?)null);

        var result = await CreateSut().ICal(1, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ICal_WhenSuccess_ReturnsFileContentResult()
    {
        var fileResult = new FileContentResult(
            System.Text.Encoding.UTF8.GetBytes("BEGIN:VCALENDAR"),
            "text/calendar; charset=utf-8")
        {
            FileDownloadName = "1.ics"
        };

        _calService.Setup(s => s.GetCalendarAsync(1, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileResult);

        var result = await CreateSut().ICal(1, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeSameAs(fileResult);
    }
}
