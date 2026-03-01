namespace LymmHolidayLets.Application.Service
{
    /// <summary>
    /// Represents an active Stripe checkout session tracked in memory cache.
    /// </summary>
    public sealed class CheckoutSession(string sessionId, DateOnly checkIn, DateOnly checkout)
    {
        public string SessionId { get; init; } = sessionId;
        public DateOnly CheckIn { get; init; } = checkIn;
        public DateOnly Checkout { get; init; } = checkout;
        public DateTime Added { get; init; } = DateTime.UtcNow;
    }
}
