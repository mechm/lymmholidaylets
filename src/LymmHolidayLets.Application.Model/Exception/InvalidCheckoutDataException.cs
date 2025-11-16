namespace LymmHolidayLets.Application.Model.Exception
{
	public sealed class InvalidCheckoutDataException : System.Exception
	{
		public InvalidCheckoutDataException(string message, System.Exception innerException)
		  : base(message, innerException)
		{
		}
	}
}
