using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Application.Interface.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LymmHolidayLets.Infrastructure.Emailer
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly SmtpConfig _smtpConfig;
        private readonly ISendGridClient _sendGridClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public SendGridEmailService(IOptions<SmtpConfig> smtpConfig, ILogger<SendGridEmailService> logger, ISendGridClient sendGridClient)
        {
            _smtpConfig = smtpConfig.Value;
            _logger = logger;
            _sendGridClient = sendGridClient;
            _retryPolicy = Policy
                .Handle<System.Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogError(ex, "Error sending email. Retrying in {RetryDelay}s", time.TotalSeconds);
                    });
        }

        public async Task SendAsync(EmailMessage emailMessage, string html)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var from = new EmailAddress(_smtpConfig.FromEmailAddress, _smtpConfig.FromName);
                var to = new EmailAddress(emailMessage.ToEmailAddress, emailMessage.ToName);
                var msg = MailHelper.CreateSingleEmail(from, to, emailMessage.Subject, "", html);

                if (emailMessage.CcEmailAddress != null)
                {
                    foreach (var cc in emailMessage.CcEmailAddress)
                    {
                        if (!string.IsNullOrEmpty(cc.Value))
                        {
                            msg.AddCc(new EmailAddress(cc.Value, cc.Key));
                        }
                    }
                }

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send email. Status code: {StatusCode}. Body: {ResponseBody}", response.StatusCode, responseBody);
                    throw new System.Exception($"Failed to send email. Status code: {response.StatusCode}");
                }
                _logger.LogInformation("Email sent to {ToEmailAddress} successfully.", emailMessage.ToEmailAddress);
            });
        }
    }
}
