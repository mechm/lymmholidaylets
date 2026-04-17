namespace LymmHolidayLets.Application.Model.Service
{
    /// <summary>
    /// Configuration options for the checkout service.
    /// Bound from the "Checkout" section of appsettings.json.
    /// </summary>
    public sealed class CheckoutOptions
    {
        /// <summary>
        /// The base URL of the site used to construct Stripe success/cancel redirect URLs.
        /// Example: "https://lymmholidaylets.com"
        /// </summary>
        public required string BaseUrl { get; init; }
    }
}
