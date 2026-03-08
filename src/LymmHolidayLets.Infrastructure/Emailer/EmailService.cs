using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace LymmHolidayLets.Infrastructure.Emailer
{
	public sealed class EmailService(IOptions<SmtpConfig> smtpConfig, ILogger<EmailService> logger) : IEmailService
	{
		private readonly SmtpConfig _smtpConfig = smtpConfig.Value;

		// TODO better to call an api endpoint to send email rather than doing it directly in the service,
		// as this would allow for better error handling and retry logic,
		// and also decouple the email sending from the rest of the application logic.
		// This would also allow for better scalability and performance, as the email
		// sending can be handled by a separate service that can be scaled independently
		// of the main application.
		public async Task SendAsync(EmailMessage emailMessage, string html)
		{
            MimeMessage message = new();

			message.From.Add(new MailboxAddress(_smtpConfig.FromName, _smtpConfig.FromEmailAddress));			
			
			message.To.Add(new MailboxAddress(emailMessage.ToName, emailMessage.ToEmailAddress));
			
			if (emailMessage.CcEmailAddress != null)
			{
				foreach (var ccEmailAddress in emailMessage.CcEmailAddress)
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
				logger.LogError(ex, "EmailService|SendAsync failed");
			}
		}
	}
}
