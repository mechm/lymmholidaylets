namespace LymmHolidayLets.Application.Model.Property
{
    /// <summary>
    /// Complete detail for a single property page — booking capacity, availability,
    /// FAQs, and aggregated guest reviews.
    /// </summary>
    public sealed class PropertyDetailResult
    {
        // ── Basic property information ────────────────────────────────────────
        public byte PropertyId { get; init; }
        public string? DisplayAddress { get; init; }
        public string? PageDescription { get; init; }

        // ── Guest capacity (drives the booking form) ──────────────────────────
        public byte MinimumNumberOfAdult { get; init; }
        public byte MaximumNumberOfGuests { get; init; }
        public byte MaximumNumberOfAdult { get; init; }
        public byte MaximumNumberOfChildren { get; init; }
        public byte MaximumNumberOfInfants { get; init; }

        /// <summary>Already-confirmed booked dates — drives the booking calendar.</summary>
        public IReadOnlyList<DateOnly> DatesBooked { get; init; } = [];

        public IReadOnlyList<PropertyFaqResult> FaQs { get; init; } = [];

        /// <summary>Null when no approved reviews exist for this property.</summary>
        public PropertyReviewAggregateResult? ReviewAggregate { get; init; }

        // ── Host information ──────────────────────────────────────────────────
        public PropertyHostResult? Host { get; init; }

        // ── Map information ───────────────────────────────────────────────────
        public PropertyMapResult? Map { get; init; }
    }

    public sealed class PropertyFaqResult
    {
        public required string Question { get; init; }
        public required string Answer { get; init; }
    }

    public sealed class PropertyHostResult
    {
        public required string Name { get; init; }
        public string? Location { get; init; }
        public byte NumberOfProperties { get; init; }
        public byte YearsExperience { get; init; }
        public required string JobTitle { get; init; }
        public string? ProfileBio { get; init; }
        public string? ImagePath { get; init; }
    }

    public sealed class PropertyMapResult
    {
        public bool ShowMap { get; init; }
        public bool ShowStreetView { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int MapZoom { get; init; }
        public double StreetViewLatitude { get; init; }
        public double StreetViewLongitude { get; init; }
        public double Pitch { get; init; }
        public double Yaw { get; init; }
        public double Zoom { get; init; }
    }

    /// <summary>
    /// Averaged ratings across all approved reviews for a property,
    /// together with the individual review records.
    /// Sub-ratings are null when no reviews carry a value for that category.
    /// </summary>
    public sealed class PropertyReviewAggregateResult
    {
        public double OverallRating { get; init; }
        public double? OverallAccuracy { get; init; }
        public double? OverallCleanliness { get; init; }
        public double? OverallCommunication { get; init; }
        public double? OverallCheckIn { get; init; }
        public double? OverallValue { get; init; }
        public double? OverallLocation { get; init; }
        public double? OverallFacilities { get; init; }
        public double? OverallComfort { get; init; }
        public IReadOnlyList<PropertyReviewResult> Reviews { get; init; } = [];
    }

    public sealed class PropertyReviewResult
    {
        public required string Name { get; init; }
        public string? Company { get; init; }
        public string? Position { get; init; }
        public required string Description { get; init; }
        public byte Rating { get; init; }
        public DateTime? DateTimeAdded { get; init; }
        public required string ReviewType { get; init; }
        public string? LinkToView { get; init; }
    }
}

