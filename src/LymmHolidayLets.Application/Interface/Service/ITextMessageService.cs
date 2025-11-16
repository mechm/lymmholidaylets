namespace LymmHolidayLets.Application.Interface.Service
{
	public interface ITextMessageService
    {
        Task SendText(string messageBody, string[] multiNumbers);

    }
}
