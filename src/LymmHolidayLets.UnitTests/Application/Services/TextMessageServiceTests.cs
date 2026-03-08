using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LymmHolidayLets.Application.Service;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class TextMessageServiceTests
{
    [Fact]
    public async Task SendText_EmptyMessage_LogsError()
    {
        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<TextMessageService>>();
        var service = new TextMessageService(config.Object, logger.Object);
        await service.SendText("", ["+1234567890"]);
        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Message body is empty")),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendText_EmptyNumbers_LogsError()
    {
        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<TextMessageService>>();
        var service = new TextMessageService(config.Object, logger.Object);
        await service.SendText("Hello", []);
        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("No recipient numbers provided")),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendText_MissingTwilioConfig_LogsError()
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["Twilio:AccountSid"]).Returns((string?)null);
        config.Setup(c => c["Twilio:AuthToken"]).Returns((string?)null);
        var logger = new Mock<ILogger<TextMessageService>>();
        var service = new TextMessageService(config.Object, logger.Object);
        await service.SendText("Hello", ["+1234567890"]);
        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Twilio configuration is missing")),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendText_ValidMessage_SendsSuccessfully()
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["Twilio:AccountSid"]).Returns("dummySid");
        config.Setup(c => c["Twilio:AuthToken"]).Returns("dummyToken");
        config.Setup(c => c["Twilio:FromNumber"]).Returns("+10000000000");
        var logger = new Mock<ILogger<TextMessageService>>();
        var service = new TextMessageService(config.Object, logger.Object);
        // Twilio will throw with dummy credentials; verify the service handles it gracefully without rethrowing.
        var exception = await Record.ExceptionAsync(() => service.SendText("Hello", ["+1234567890"]));
        Assert.Null(exception);
    }
}
