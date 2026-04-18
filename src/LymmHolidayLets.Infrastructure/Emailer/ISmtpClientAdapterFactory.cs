namespace LymmHolidayLets.Infrastructure.Emailer;

public interface ISmtpClientAdapterFactory
{
    ISmtpClientAdapter Create();
}
