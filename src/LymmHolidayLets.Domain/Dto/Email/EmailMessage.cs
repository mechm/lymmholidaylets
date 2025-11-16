
namespace LymmHolidayLets.Domain.Dto.Email
{
	public sealed class EmailMessage
	{
		public string ToName { get; set; } = null!;
		public string ToEmailAddress { get; set; } = null!;
		public IDictionary<string, string?>? CcEmailAddress { get; set; }
		public string Subject { get; set; } = null!;
	}
}
