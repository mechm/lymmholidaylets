using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using LymmHolidayLets.Domain.ReadModel.Email;
using Microsoft.Extensions.Configuration;

namespace LymmHolidayLets.Application.Service
{
    public sealed class CustomerBookingConfirmationBuilder(
        ICustomerBookingEmailDataAdapter customerBookingEmailDataAdapter,
        IDapperPriceDataAdapter priceDataAdapter,
        IConfiguration configuration) : ICustomerBookingConfirmationBuilder
    {
        private readonly string _siteBaseUrl = configuration["Keys:SiteMaps"] ?? "https://lymmholidaylets.com";

        public Task<BookingConfirmationForCustomer> BuildAsync(BookingNotificationRequested notificationRequested)
        {
            var propertyEmailData = customerBookingEmailDataAdapter.GetByPropertyId(notificationRequested.PropertyId)
                ?? throw new InvalidOperationException(
                    $"Booking confirmation email content is not configured for property {notificationRequested.PropertyId}.");

            var priceDetail = priceDataAdapter.GetPriceDetail(
                notificationRequested.PropertyId,
                notificationRequested.CheckIn,
                notificationRequested.CheckOut);

            return Task.FromResult(new BookingConfirmationForCustomer
            {
                PropertyName = propertyEmailData.PropertyName,
                BookingReference = FormatBookingReference(notificationRequested.BookingReference),
                PropertyUrl = $"{_siteBaseUrl.TrimEnd('/')}/property/detail/{notificationRequested.PropertyId}",
                Bedroom = propertyEmailData.Bedroom,
                Bathroom = propertyEmailData.Bathroom,
                CheckIn = notificationRequested.CheckIn,
                CheckOut = notificationRequested.CheckOut,
                NoAdult = notificationRequested.NoAdult,
                NoChildren = notificationRequested.NoChildren,
                NoInfant = notificationRequested.NoInfant,
                Name = notificationRequested.Name,
                Email = notificationRequested.Email ?? string.Empty,
                Telephone = notificationRequested.Telephone ?? string.Empty,
                PostalCode = notificationRequested.PostalCode ?? string.Empty,
                Country = notificationRequested.Country ?? string.Empty,
                Total = notificationRequested.AmountTotal,
                CheckInTimeAfter = propertyEmailData.CheckInTimeAfter,
                CheckOutTimeBefore = propertyEmailData.CheckOutTimeBefore,
                FullAddress = FormatAddress(propertyEmailData),
                DirectionsUrl = propertyEmailData.DirectionsUrl,
                ArrivalInstructions = propertyEmailData.ArrivalInstructions,
                HeroImageUrl = ResolveUrl(propertyEmailData.HeroImagePath),
                HeroImageAltText = propertyEmailData.HeroImageAltText,
                HouseRules = propertyEmailData.HouseRules.Select(static rule => rule.Text).ToList(),
                SafetyItems = propertyEmailData.SafetyItems.Select(static item => item.Text).ToList(),
                CancellationPolicyText = ResolveCancellationPolicy(notificationRequested.CheckIn, propertyEmailData.CancellationPolicies),
                PaymentLines = BuildPaymentLines(notificationRequested.CheckIn, notificationRequested.CheckOut, priceDetail)
            });
        }

        private static string FormatAddress(CustomerBookingEmailData propertyEmailData)
        {
            var parts = new[]
            {
                propertyEmailData.AddressLineOne,
                propertyEmailData.AddressLineTwo,
                propertyEmailData.TownOrCity,
                propertyEmailData.County,
                propertyEmailData.Postcode,
                propertyEmailData.Country
            };

            return string.Join(", ", parts.Where(static part => !string.IsNullOrWhiteSpace(part)));
        }

        private static string ResolveCancellationPolicy(
            DateOnly checkIn,
            IReadOnlyList<CustomerBookingEmailCancellationPolicy> cancellationPolicies)
        {
            if (cancellationPolicies.Count == 0)
            {
                throw new InvalidOperationException("At least one cancellation policy must be configured for the property.");
            }

            var daysUntilCheckIn = checkIn.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
            var threshold = cancellationPolicies
                .Select(static policy => policy.DaysBeforeCheckIn)
                .Distinct()
                .OrderBy(static value => value)
                .Select(static value => (short?)value)
                .FirstOrDefault(value => value.HasValue && daysUntilCheckIn < value.Value)
                ?? cancellationPolicies.Max(static policy => policy.DaysBeforeCheckIn);

            return string.Join(
                Environment.NewLine,
                cancellationPolicies
                    .Where(policy => policy.DaysBeforeCheckIn == threshold)
                    .OrderBy(static policy => policy.SequenceOrder)
                    .Select(static policy => policy.PolicyText));
        }

        private IReadOnlyList<BookingConfirmationPaymentLine> BuildPaymentLines(
            DateOnly checkIn,
            DateOnly checkOut,
            PriceAggregate priceDetail)
        {
            var lines = new List<BookingConfirmationPaymentLine>();
            var nights = checkOut.DayNumber - checkIn.DayNumber;

            if (priceDetail.TotalNightlyPrice.HasValue)
            {
                lines.Add(new BookingConfirmationPaymentLine(
                    nights == 1 ? "1 night" : $"{nights} nights",
                    priceDetail.TotalNightlyPrice.Value));
            }

            lines.AddRange(priceDetail.AdditionalProduct.Select(product =>
                new BookingConfirmationPaymentLine(
                    product.Quantity > 1 ? $"{product.StripeName} x {product.Quantity}" : product.StripeName,
                    product.StripeDefaultUnitPrice * product.Quantity)));

            return lines;
        }

        private string? ResolveUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? path
                : $"{_siteBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        private static string? FormatBookingReference(string? sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return null;

            // Strip cs_test_ / cs_live_ prefix, take last 12 chars and uppercase
            var prefixes = new[] { "cs_test_", "cs_live_", "cs_" };
            var raw = sessionId;
            foreach (var prefix in prefixes)
            {
                if (raw.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    raw = raw[prefix.Length..];
                    break;
                }
            }

            var trimmed = raw.Length > 12 ? raw[^12..] : raw;
            return "LHL-" + trimmed.ToUpperInvariant();
        }
    }
}
