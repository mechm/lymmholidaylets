using FluentAssertions;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Infrastructure.Emailer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Infrastructure.Emailer;

public class EmailServiceTests
{
    private readonly Mock<ISmtpClientAdapterFactory> _smtpClientFactory = new();
    private readonly Mock<ISmtpClientAdapter> _smtpClient = new();
    private readonly Mock<ILogger<EmailService>> _logger = new();

    private EmailService CreateSut() => new(
        Options.Create(new SmtpConfig
        {
            FromName = "Lymm Holiday Lets",
            FromEmailAddress = "bookings@lymmholidaylets.com",
            Server = "smtp.example.com",
            EnableSsl = true,
            User = "smtp-user",
            Password = "smtp-password",
            Port = 465
        }),
        _smtpClientFactory.Object,
        _logger.Object);

    [Fact]
    public async Task SendAsync_WhenAuthenticateFails_RethrowsException()
    {
        var expected = new InvalidOperationException("smtp fail");

        _smtpClientFactory.Setup(x => x.Create()).Returns(_smtpClient.Object);
        _smtpClient
            .Setup(x => x.ConnectAsync("smtp.example.com", 465, true, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _smtpClient
            .Setup(x => x.AuthenticateAsync("smtp-user", "smtp-password", It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var act = () => CreateSut().SendAsync(
            new EmailMessage
            {
                ToName = "Test Guest",
                ToEmailAddress = "guest@example.com",
                Subject = "Test subject"
            },
            "<p>Hello</p>");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("smtp fail");

        _smtpClient.Verify(
            x => x.SendAsync(It.IsAny<MimeKit.MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
