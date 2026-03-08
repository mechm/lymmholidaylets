namespace LymmHolidayLets.Application.Model.Service
{
    /// <summary>
    /// Application-level result of a checkout operation.
    /// Decouples the Application layer from Stripe SDK types.
    /// </summary>
    public sealed class CheckoutResult
    {
        public required string SessionId { get; init; }
        public required string SessionUrl { get; init; }
        public DateOnly CheckIn { get; init; }
        public DateOnly CheckOut { get; init; }
    }
}
