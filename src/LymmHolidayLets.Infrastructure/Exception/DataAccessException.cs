namespace LymmHolidayLets.Infrastructure.Exception
{
	public sealed class DataAccessException : System.Exception
	{
		public DataAccessException(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
