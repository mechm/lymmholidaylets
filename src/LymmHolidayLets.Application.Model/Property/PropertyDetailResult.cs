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
        public string? Description { get; init; }
        public string? MetaDescription { get; init; }
        public string? Slug { get; init; }

        // ── Guest capacity (drives the booking form) ──────────────────────────
        public byte MinimumNumberOfAdult { get; init; }
        public byte MaximumNumberOfGuests { get; init; }
        public byte MaximumNumberOfAdult { get; init; }
        public byte MaximumNumberOfChildren { get; init; }
        public byte MaximumNumberOfInfants { get; init; }

        // ── Room counts ───────────────────────────────────────────────────────
        public byte NumberOfBedrooms { get; init; }
        /// <summary>Total bathrooms. Half-values (e.g. 1.5) indicate an additional en-suite.</summary>
        public double NumberOfBathrooms { get; init; }
        public byte NumberOfReceptionRooms { get; init; }
        public byte NumberOfKitchens { get; init; }
        public byte NumberOfCarSpaces { get; init; }

        // ── Booking rules ─────────────────────────────────────────────────────
        /// <summary>Earliest time guests may check in (e.g. 15:00).</summary>
        public TimeOnly CheckInTime { get; init; }
        /// <summary>Latest time guests must check out (e.g. 10:00).</summary>
        public TimeOnly CheckOutTime { get; init; }
        /// <summary>Minimum number of nights per booking.</summary>
        public byte MinimumStayNights { get; init; }
        /// <summary>Maximum number of nights per booking. Null means no upper limit.</summary>
        public short? MaximumStayNights { get; init; }

        /// <summary>Already-confirmed booked dates — drives the booking calendar.</summary>
        public IReadOnlyList<DateOnly> DatesBooked { get; init; } = [];

        public IReadOnlyList<PropertyFaqResult> Faqs { get; init; } = [];

        /// <summary>Null when no approved reviews exist for this property.</summary>
        public PropertyRatingSummaryResult? RatingSummary { get; init; }

        // ── Host information ──────────────────────────────────────────────────
        public PropertyHostResult? Host { get; init; }

        // ── Map information ───────────────────────────────────────────────────
        public PropertyMapResult? Map { get; init; }

        // ── Amenities ─────────────────────────────────────────────────────────
        public IReadOnlyList<string> Amenities { get; init; } = [];

        // ── Reviews (capped at 10 in detail endpoint) ─────────────────────────
        public IReadOnlyList<PropertyReviewResult> Reviews { get; init; } = [];

        // ── Images ────────────────────────────────────────────────────────────
        public IReadOnlyList<PropertyImageResult> Images { get; init; } = [];

        // ── Bedrooms ──────────────────────────────────────────────────────────
        public IReadOnlyList<PropertyBedroomResult> Bedrooms { get; init; } = [];

        /// <summary>When this property record was last modified. Used for HTTP cache validation.</summary>
        public DateTimeOffset? LastModified { get; init; }

        /// <summary>
        /// When calendar availability was last changed by the iCal importer.
        /// Used by the API cache layer to detect stale cached property detail.
        /// </summary>
        public DateTime? CalendarLastModified { get; init; }

        /// <summary>Embedded video HTML (e.g. YouTube iframe). Null when no video is set.</summary>
        public string? VideoHtml { get; init; }

        /// <summary>Property disclaimer text. Null when no disclaimer is set.</summary>
        public string? Disclaimer { get; init; }
    }

    public sealed class PropertyBedroomResult
    {
        public byte BedroomNumber { get; init; }
        public string? BedroomName { get; init; }
        public required string BedType { get; init; }
        public string? BedTypeIcon { get; init; }
        public byte NumberOfBeds { get; init; }
    }

    public sealed class PropertyImageResult
    {
        public required string ImagePath { get; init; }
        public string? AltText { get; init; }
        public byte SequenceOrder { get; init; }
    }

    public sealed class PropertyFaqResult
    {
        public required string Question { get; init; }
        public required string Answer { get; init; }
    }

    public sealed class PropertyHostResult
    {
        public required string Name { get; init; }
        public byte NumberOfProperties { get; init; }
        public byte YearsExperience { get; init; }
        public required string JobTitle { get; init; }
        public string? ProfileBio { get; init; }
        public string? Location { get; init; }
        public string? ImagePath { get; init; }
    }

    public sealed class PropertyMapResult
    {
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
    /// Averaged ratings across all approved reviews for a property.
    /// Sub-ratings are null when no reviews carry a value for that category.
    /// </summary>
    public sealed class PropertyRatingSummaryResult
    {
        public double Rating { get; init; }
        public double? Accuracy { get; init; }
        public double? Cleanliness { get; init; }
        public double? Communication { get; init; }
        public double? CheckInExperience { get; init; }
        public double? Value { get; init; }
        public double? Location { get; init; }
        public double? Facilities { get; init; }
        public double? Comfort { get; init; }
        /// <summary>Total number of approved reviews regardless of how many are returned in the detail response.</summary>
        public int TotalReviews { get; init; }
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

