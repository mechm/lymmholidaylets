namespace LymmHolidayLets.Application.Model.Service;

public sealed class StripeSessionResult
{
    public required string Id { get; init; }
    public string? Url { get; init; }
}

public sealed class StripeProductResult
{
    public required string Id { get; init; }
    public required string DefaultPriceId { get; init; }
}

public sealed class StripeCouponResult
{
    public required string Id { get; init; }
    public decimal? PercentOff { get; init; }
}

public sealed class StripeProductAndCouponResult
{
    public required StripeProductResult Product { get; init; }
    public StripeCouponResult? Coupon { get; init; }
}

public sealed class ParsedStripeWebhookEvent
{
    public required string EventId { get; init; }
    public required string EventType { get; init; }
    public ParsedStripeCheckoutSession? CheckoutSession { get; init; }
}

public sealed class ParsedStripeCheckoutSession
{
    public required string SessionId { get; init; }
    public required string PaymentStatus { get; init; }
    public required IReadOnlyDictionary<string, string> Metadata { get; init; }
    public ParsedStripeCustomerDetails CustomerDetails { get; init; } = new();
    public long? AmountTotal { get; init; }
}

public sealed class ParsedStripeCustomerDetails
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
}
