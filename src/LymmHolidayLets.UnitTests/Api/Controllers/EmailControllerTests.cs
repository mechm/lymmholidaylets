using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class EmailControllerTests
{
    private readonly Mock<IEmailEnquiryService> _emailService = new();
    private readonly Mock<IRecaptchaValidationService> _recaptcha = new();
    private readonly Mock<ILogger<EmailController>> _logger = new();
    private readonly EmailController _sut;

    public EmailControllerTests()
    {
        _sut = new EmailController(_emailService.Object, _recaptcha.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    private static EmailEnquiryRequest ValidRequest() => new()
    {
        Name = "Jane Smith",
        EmailAddress = "jane@example.com",
        Message = "I'd like to book.",
        ReCaptchaToken = "valid-token"
    };

    [Fact]
    public async Task Submit_WhenRecaptchaFails_ReturnsBadRequest()
    {
        _recaptcha.Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Submit(ValidRequest(), CancellationToken.None);

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var body = bad.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Submit_WhenProcessingFails_Returns500()
    {
        _recaptcha.Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _emailService.Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquiryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Submit(ValidRequest(), CancellationToken.None);

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Submit_WhenSuccess_ReturnsOk()
    {
        _recaptcha.Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _emailService.Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquiryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Submit(ValidRequest(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeTrue();
    }
}
