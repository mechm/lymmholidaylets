using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using LymmHolidayLets.Application.Service;

namespace LymmHolidayLets.Api.Tests
{
    public class TextMessageServiceTests
    {
        [Fact]
        public async Task SendText_EmptyMessage_LogsError()
        {
            var config = new Mock<IConfiguration>();
            var logger = new Mock<Domain.Interface.ILogger>();
            var service = new TextMessageService(config.Object, logger.Object);
            await service.SendText("", new[] { "+1234567890" });
            logger.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Message body is empty"))), Times.Once);
        }

        [Fact]
        public async Task SendText_EmptyNumbers_LogsError()
        {
            var config = new Mock<IConfiguration>();
            var logger = new Mock<Domain.Interface.ILogger>();
            var service = new TextMessageService(config.Object, logger.Object);
            await service.SendText("Hello", new string[0]);
            logger.Verify(l => l.LogError(It.Is<string>(s => s.Contains("No recipient numbers provided"))), Times.Once);
        }

        [Fact]
        public async Task SendText_MissingTwilioConfig_LogsError()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Twilio:AccountSid"]).Returns((string)null);
            config.Setup(c => c["Twilio:AuthToken"]).Returns((string)null);
            var logger = new Mock<Domain.Interface.ILogger>();
            var service = new TextMessageService(config.Object, logger.Object);
            await service.SendText("Hello", new[] { "+1234567890" });
            logger.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Twilio configuration is missing"))), Times.Once);
        }

        [Fact]
        public async Task SendText_ValidMessage_SendsSuccessfully()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Twilio:AccountSid"]).Returns("dummySid");
            config.Setup(c => c["Twilio:AuthToken"]).Returns("dummyToken");
            config.Setup(c => c["Twilio:FromNumber"]).Returns("+10000000000");
            var logger = new Mock<Domain.Interface.ILogger>();
            var service = new TextMessageService(config.Object, logger.Object);
            // This test assumes SendText returns true/false or throws on error. If it doesn't, just verify no error is logged.
            await service.SendText("Hello", new[] { "+1234567890" });
            logger.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
