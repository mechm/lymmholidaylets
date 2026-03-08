using LymmHolidayLets.Application.Interface.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LymmHolidayLets.Application.Service
{
    public sealed class TextMessageService(IConfiguration config, ILogger<TextMessageService> logger) : ITextMessageService
    {
        private const string DefaultFromNumber = "+447897031197";

        public async Task SendText(string messageBody, string[] multiNumbers)
        {
            if (string.IsNullOrWhiteSpace(messageBody))
            {
                logger.LogError("TextMessageService|SendText|Message body is empty");
                return;
            }

            if (multiNumbers.Length == 0)
            {
                logger.LogError("TextMessageService|SendText|No recipient numbers provided");
                return;
            }

            var accountSid = config["Twilio:AccountSid"];
            var authToken = config["Twilio:AuthToken"];
            var fromNumber = config["Twilio:FromNumber"] ?? DefaultFromNumber;

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            {
                logger.LogError("TextMessageService|SendText|Twilio configuration is missing (AccountSid or AuthToken)");
                return;
            }

            // Ideally TwilioClient.Init would be called once at startup in Program.cs
            TwilioClient.Init(accountSid, authToken);

            var sendTasks = multiNumbers
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(async number =>
                {
                    try
                    {
                        var options = new CreateMessageOptions(new PhoneNumber(number))
                        {
                            From = new PhoneNumber(fromNumber),
                            Body = messageBody
                        };

                        var result = await MessageResource.CreateAsync(options);

                        if (result.ErrorCode.HasValue)
                        {
                            logger.LogError("Twilio Error for {Number}: {ErrorMessage} (Code: {ErrorCode})", number, result.ErrorMessage, result.ErrorCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "TextMessageService|SendText|Exception for {Number}", number);
                    }
                });

            await Task.WhenAll(sendTasks);
        }
    }
}
