using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class EmailControllerTests
{
    private readonly Mock<IEmailEnquiryProcessingService> _emailService = new();
    private readonly Mock<ILogger<EmailController>> _logger = new();
    private readonly EmailController _sut;

    public EmailControllerTests()
    {
        _sut = new EmailController(_emailService.Object, _logger.Object)
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
        _emailService.Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquirySubmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(EmailEnquiryResponse.Failure("Security verification failed. Please try again."));

        var result = await _sut.Submit(ValidRequest(), CancellationToken.None);

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var body = bad.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Submit_WhenProcessingThrows_Returns500AndLogsWithContext()
    {
        _emailService.Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquirySubmission>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.Submit(ValidRequest(), CancellationToken.None);

        // Returns 500 with the standard failure response shape
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var body = statusResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeFalse();

        // Logs the failure with request context so the caller is identifiable in logs
        _logger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Email enquiry processing failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Submit_WhenSuccess_ReturnsOk()
    {
        _emailService.Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquirySubmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(EmailEnquiryResponse.Success());

        var result = await _sut.Submit(ValidRequest(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeTrue();
    }
}
