namespace LymmHolidayLets.NotificationWorker.Services
{
	public interface ITextMessageService
    {
        Task SendText(string messageBody, string[] multiNumbers);

    }
}
