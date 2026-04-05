using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Application.Service;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class TextMessageServiceTests
{
    private static IOptions<TwilioOptions> DefaultOptions() =>
        Microsoft.Extensions.Options.Options.Create(new TwilioOptions
        {
            AccountSid = "ACdummy",
            AuthToken  = "dummyToken",
            FromNumber = "+10000000000"
        });

    private static TextMessageService CreateSut(IOptions<TwilioOptions>? options = null) =>
        new(options ?? DefaultOptions(), new Mock<ILogger<TextMessageService>>().Object);

    [Fact]
    public async Task SendText_EmptyMessage_LogsError()
    {
        var logger = new Mock<ILogger<TextMessageService>>();
        var sut = new TextMessageService(DefaultOptions(), logger.Object);

        await sut.SendText("", ["+1234567890"]);

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
        var logger = new Mock<ILogger<TextMessageService>>();
        var sut = new TextMessageService(DefaultOptions(), logger.Object);

        await sut.SendText("Hello", []);

        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("No recipient numbers provided")),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendText_ValidMessage_DoesNotThrow()
    {
        // Twilio will throw with dummy credentials because TwilioClient.Init is not called here.
        // The service must catch that exception and log it rather than letting it propagate.
        var sut = CreateSut();
        var exception = await Record.ExceptionAsync(() => sut.SendText("Hello", ["+1234567890"]));
        Assert.Null(exception);
    }
}
