namespace LymmHolidayLets.Application.Model.Exception
{
	public sealed class InvalidCheckoutDataException(string message, System.Exception innerException)
		: System.Exception(message, innerException);
}
