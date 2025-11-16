namespace LymmHolidayLets.Domain.Dto.Email
{
	public sealed class SmtpConfig
	{
		public string FromName { get; set; } = null!;
		public string FromEmailAddress { get; set; } = null!;
		public string Server { get; set; } = null!;
		public bool EnableSsl { get; set; }
		public string User { get; set; } = null!;
		public string Password { get; set; } = null!;
		public int Port { get; set; }
	}
}
