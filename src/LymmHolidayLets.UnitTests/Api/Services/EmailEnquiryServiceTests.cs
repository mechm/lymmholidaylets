using FluentAssertions;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Application.Service;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class EmailEnquiryProcessingServiceTests
{
    private readonly Mock<IEmailEnquiryCommand> _emailEnquiryCommand = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();
    private readonly Mock<IRecaptchaValidationService> _recaptchaValidationService = new();
    private readonly Mock<ILogger<EmailEnquiryProcessingService>> _logger = new();

    private EmailEnquiryProcessingService CreateSut() => new(
        _emailEnquiryCommand.Object,
        _publishEndpoint.Object,
        _recaptchaValidationService.Object,
        _logger.Object);

    private static EmailEnquirySubmission ValidRequest() => new()
    {
        Name = "Jane Smith",
        EmailAddress = "jane@example.com",
        Message = "I'd like to book for summer 2026.",
        ReCaptchaToken = "valid-token",
        ClientIp = "127.0.0.1"
    };

    [Fact]
    public async Task ProcessEnquiryAsync_ValidRequest_SavesEnquiryToDatabase()
    {
        _recaptchaValidationService.Setup(s => s.ValidateAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var sut = CreateSut();

        await sut.ProcessEnquiryAsync(ValidRequest());

        _emailEnquiryCommand.Verify(
            c => c.Create(It.Is<LymmHolidayLets.Application.Model.Command.EmailEnquiry>(e =>
                e.Name == "Jane Smith" && e.EmailAddress == "jane@example.com")),
            Times.Once);
    }

    [Fact]
    public async Task ProcessEnquiryAsync_ValidRequest_PublishesEvent()
    {
        _recaptchaValidationService.Setup(s => s.ValidateAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        await CreateSut().ProcessEnquiryAsync(ValidRequest());

        _publishEndpoint.Verify(
            p => p.Publish(
                It.Is<Contracts.EmailEnquirySubmittedEvent>(e =>
                    e.Name == "Jane Smith" && e.EmailAddress == "jane@example.com"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessEnquiryAsync_WhenCommandThrows_PropagatesException()
    {
        _recaptchaValidationService.Setup(s => s.ValidateAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _emailEnquiryCommand
            .Setup(c => c.Create(It.IsAny<LymmHolidayLets.Application.Model.Command.EmailEnquiry>()))
            .Throws(new Exception("DB error"));

        var act = () => CreateSut().ProcessEnquiryAsync(ValidRequest());

        await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
    }

    [Fact]
    public async Task ProcessEnquiryAsync_WhenRecaptchaFails_ReturnsFailure()
    {
        _recaptchaValidationService.Setup(s => s.ValidateAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await CreateSut().ProcessEnquiryAsync(ValidRequest());

        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be("Security verification failed. Please try again.");
        _emailEnquiryCommand.Verify(c => c.Create(It.IsAny<LymmHolidayLets.Application.Model.Command.EmailEnquiry>()), Times.Never);
    }
}
