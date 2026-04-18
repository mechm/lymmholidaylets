using System.Net.Security;
using MailKit.Net.Smtp;
using MimeKit;

namespace LymmHolidayLets.Infrastructure.Emailer;

public sealed class MailKitSmtpClientAdapter : ISmtpClientAdapter
{
    private readonly SmtpClient _smtpClient = new();

    public RemoteCertificateValidationCallback? ServerCertificateValidationCallback
    {
        get => _smtpClient.ServerCertificateValidationCallback;
        set => _smtpClient.ServerCertificateValidationCallback = value;
    }

    public Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default) =>
        _smtpClient.ConnectAsync(host, port, useSsl, cancellationToken);

    public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default) =>
        _smtpClient.AuthenticateAsync(userName, password, cancellationToken);

    public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default) =>
        _smtpClient.SendAsync(message, cancellationToken);

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default) =>
        _smtpClient.DisconnectAsync(quit, cancellationToken);

    public void Dispose() => _smtpClient.Dispose();
}
