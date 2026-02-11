using LymmHolidayLets.Application.Interface.Service;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace LymmHolidayLets.Application.Service
{
    public sealed class TextMessageService(IConfiguration config, Domain.Interface.ILogger logger) : ITextMessageService
    {
        public Task SendText(string messageBody, string[] multiNumbers)
		{
            try
            {
                foreach (var t in multiNumbers)
                {
                    TwilioClient.Init(config["Twilio:AccountSid"], config["Twilio:AuthToken"]);

                    var messageOptions = new CreateMessageOptions(
                        new PhoneNumber(t))
                    {
                        From = new PhoneNumber("+447897031197"),
                        Body = messageBody
                    };

                    var output = MessageResource.Create(messageOptions);

                    if (output.Status == MessageResource.StatusEnum.Failed)
                    {
                        logger.LogError("Unable to send text");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"TextMessageService|SendText|{ex.Message}");
            }

            return Task.CompletedTask;
        }
	}
}
