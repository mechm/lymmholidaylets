using System.Net.Security;
using MimeKit;

namespace LymmHolidayLets.Infrastructure.Emailer;

public interface ISmtpClientAdapter : IDisposable
{
    RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

    Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default);

    Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default);

    Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);

    Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default);
}
