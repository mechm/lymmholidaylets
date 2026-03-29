namespace LymmHolidayLets.Api.Models.Checkout;

/// <summary>
/// The payload returned when a Stripe Checkout session is successfully created.
/// </summary>
/// <param name="Url">The Stripe-hosted checkout URL the client should redirect to.</param>
public sealed record CheckoutSessionResponse(string Url);

