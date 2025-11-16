using LymmHolidayLets.Application.Interface.Service;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace LymmHolidayLets.Application.Service
{
    public sealed class TextMessageService : ITextMessageService
    {
        private readonly IConfiguration _config;
        private readonly Domain.Interface.ILogger _logger;

        public TextMessageService(IConfiguration config, Domain.Interface.ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task SendText(string messageBody, string[] multiNumbers)
		{
            try
            {
                foreach (var t in multiNumbers)
                {
                    TwilioClient.Init(_config["Twilio:AccountSid"], _config["Twilio:AuthToken"]);

                    var messageOptions = new CreateMessageOptions(
                        new PhoneNumber(t))
                    {
                        From = new PhoneNumber("+447897031197"),
                        Body = messageBody
                    };

                    MessageResource output = MessageResource.Create(messageOptions);

                    if (output.Status == MessageResource.StatusEnum.Failed)
                    {
                        _logger.LogError("Unable to send text");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TextMessageService|SendText|{ex.Message}");
            }

            return Task.CompletedTask;
        }
	}
}
