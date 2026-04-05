namespace LymmHolidayLets.Infrastructure.Exception
{
	public sealed class DataAccessException(string message, System.Exception innerException)
		: System.Exception(message, innerException);
}
