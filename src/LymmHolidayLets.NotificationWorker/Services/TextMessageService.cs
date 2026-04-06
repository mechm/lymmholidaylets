using LymmHolidayLets.NotificationWorker.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LymmHolidayLets.NotificationWorker.Services
{
    /// <summary>
    /// Sends SMS messages via Twilio to one or more recipient numbers.
    /// The Twilio client is initialised once at application startup via
    /// <c>TwilioClient.Init</c> in <c>Program.cs</c> — this service assumes
    /// it is already initialised and does not call <c>Init</c> itself.
    /// </summary>
    public sealed class TextMessageService(
        IOptions<TwilioOptions> options,
        ILogger<TextMessageService> logger) : ITextMessageService
    {
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

            var fromNumber = options.Value.FromNumber;

            var sendTasks = multiNumbers
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(async number =>
                {
                    try
                    {
                        var messageOptions = new CreateMessageOptions(new PhoneNumber(number))
                        {
                            From = new PhoneNumber(fromNumber),
                            Body = messageBody
                        };

                        var result = await MessageResource.CreateAsync(messageOptions);

                        if (result.ErrorCode.HasValue)
                        {
                            logger.LogError(
                                "Twilio error for {Number}: {ErrorMessage} (Code: {ErrorCode})",
                                number, result.ErrorMessage, result.ErrorCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "TextMessageService|SendText|Exception sending to {Number}", number);
                    }
                });

            await Task.WhenAll(sendTasks);
        }
    }
}
