using LymmHolidayLets.Domain.Dto.Email;

namespace LymmHolidayLets.Application.Interface.Service
{
	public interface IEmailService
	{
		Task SendAsync(EmailMessage emailMessage, string html);
	}
}
