using FluentAssertions;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Api.Services;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Domain.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Services;

public class EmailEnquiryServiceTests
{
    private readonly Mock<IEmailEnquiryCommand> _emailEnquiryCommand = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IEmailTemplateBuilder> _emailTemplateBuilder = new();
    private readonly Mock<IConfiguration> _config = new();
    private readonly Mock<ILogger<EmailEnquiryService>> _logger = new();

    private EmailEnquiryService CreateSut() => new(
        _emailEnquiryCommand.Object,
        _emailService.Object,
        _emailTemplateBuilder.Object,
        _config.Object,
        _logger.Object);

    private static EmailEnquiryRequest ValidRequest() => new()
    {
        Name = "Jane Smith",
        EmailAddress = "jane@example.com",
        Message = "I'd like to book for summer 2026.",
        ReCaptchaToken = "valid-token"
    };

    [Fact]
    public async Task ProcessEnquiryAsync_ValidRequest_SavesEnquiryToDatabase()
    {
        var sut = CreateSut();

        await sut.ProcessEnquiryAsync(ValidRequest());

        _emailEnquiryCommand.Verify(
            c => c.Create(It.Is<LymmHolidayLets.Application.Model.Command.EmailEnquiry>(e =>
                e.Name == "Jane Smith" && e.EmailAddress == "jane@example.com")),
            Times.Once);
    }

    [Fact]
    public async Task ProcessEnquiryAsync_ValidRequest_ReturnsTrue()
    {
        var result = await CreateSut().ProcessEnquiryAsync(ValidRequest());

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessEnquiryAsync_WhenCommandThrows_ReturnsFalse()
    {
        _emailEnquiryCommand
            .Setup(c => c.Create(It.IsAny<LymmHolidayLets.Application.Model.Command.EmailEnquiry>()))
            .Throws(new Exception("DB error"));

        var result = await CreateSut().ProcessEnquiryAsync(ValidRequest());

        result.Should().BeFalse();
    }
}
