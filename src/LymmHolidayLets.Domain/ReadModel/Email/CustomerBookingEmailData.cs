namespace LymmHolidayLets.Domain.ReadModel.Email
{
    public sealed class CustomerBookingEmailData
    {
        public required byte PropertyId { get; init; }
        public required string PropertyName { get; init; }
        public byte? Bedroom { get; init; }
        public double? Bathroom { get; init; }
        public TimeOnly CheckInTimeAfter { get; init; }
        public TimeOnly CheckOutTimeBefore { get; init; }
        public string? AddressLineOne { get; init; }
        public string? AddressLineTwo { get; init; }
        public string? TownOrCity { get; init; }
        public string? County { get; init; }
        public string? Postcode { get; init; }
        public string? Country { get; init; }
        public string? DirectionsUrl { get; init; }
        public string? ArrivalInstructions { get; init; }
        public string? HeroImagePath { get; init; }
        public string? HeroImageAltText { get; init; }
        public IReadOnlyList<CustomerBookingEmailTextItem> HouseRules { get; init; } = [];
        public IReadOnlyList<CustomerBookingEmailTextItem> SafetyItems { get; init; } = [];
        public IReadOnlyList<CustomerBookingEmailCancellationPolicy> CancellationPolicies { get; init; } = [];
    }

    public sealed class CustomerBookingEmailTextItem
    {
        public required string Text { get; init; }
        public byte SequenceOrder { get; init; }
    }

    public sealed class CustomerBookingEmailCancellationPolicy
    {
        public short DaysBeforeCheckIn { get; init; }
        public required string PolicyText { get; init; }
        public byte SequenceOrder { get; init; }
    }
}
