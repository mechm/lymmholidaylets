using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Infrastructure.Emailer
{
	public sealed class EmailService : IEmailService
	{
		private readonly ILogger _logger;
		private readonly SmtpConfig _smtpConfig;

		public EmailService(IOptions<SmtpConfig> smtpConfig, ILogger logger)
		{
			_smtpConfig = smtpConfig.Value;
			_logger = logger;
		}

		public async Task SendAsync(EmailMessage emailMessage, string html)
		{
            MimeMessage message = new();

			message.From.Add(new MailboxAddress(_smtpConfig.FromName, _smtpConfig.FromEmailAddress));			
			
			message.To.Add(new MailboxAddress(emailMessage.ToName, emailMessage.ToEmailAddress));
			
			if (emailMessage.CcEmailAddress != null)
			{
				foreach (KeyValuePair<string,string?> ccEmailAddress in emailMessage.CcEmailAddress)
				{
					if (!string.IsNullOrEmpty(ccEmailAddress.Value))
					{
						message.Cc.Add(new MailboxAddress(ccEmailAddress.Key, ccEmailAddress.Value));
					}
				}
			}

			message.Subject = emailMessage.Subject;

			try
			{
				message.Body = new TextPart("html") { Text = html };

				using var client = new SmtpClient();
				await client.ConnectAsync(_smtpConfig.Server, _smtpConfig.Port, _smtpConfig.EnableSsl).ConfigureAwait(false);
				await client.AuthenticateAsync(_smtpConfig.User, _smtpConfig.Password);
				await client.SendAsync(message).ConfigureAwait(false);
				await client.DisconnectAsync(true).ConfigureAwait(false);
			}
			catch (System.Exception ex)
			{
				_logger.LogError($"EmailService|SendAsync|{ex.Message}");
			}
		}
	}
}
