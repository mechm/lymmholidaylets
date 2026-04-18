namespace LymmHolidayLets.Infrastructure.Emailer;

public sealed class MailKitSmtpClientAdapterFactory : ISmtpClientAdapterFactory
{
    public ISmtpClientAdapter Create() => new MailKitSmtpClientAdapter();
}
