namespace LymmHolidayLets.Application.Model.Service
{
    public sealed class Product
    {
        private static string GetProductName(string friendlyName, DateTime checkIn, DateTime checkout)
        {
            return $"{friendlyName} - {checkIn.Date:dd/MM/yyyy} to {checkout.Date:dd/MM/yyyy}";
        }

        private static string GetProductDescription(DateTime checkIn, DateTime checkout)
        {
            return $"Price for {(checkout - checkIn).Days} Nights";
        }
    }
}
