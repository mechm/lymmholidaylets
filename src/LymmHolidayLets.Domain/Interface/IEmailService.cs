using LymmHolidayLets.Domain.Dto.Email;

namespace LymmHolidayLets.Domain.Interface
{
	public interface IEmailService
	{
		Task SendAsync(EmailMessage emailMessage, string html);
	}
}
