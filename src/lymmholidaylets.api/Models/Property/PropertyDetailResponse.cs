using System.Text.Json.Serialization;
using LymmHolidayLets.Api.Converters;
using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Api.Models.Property;

/// <summary>
/// Flat API response for property detail. All PropertyDetailResult fields are promoted to root level
/// (except MetaDescription which is used internally by SeoMetaGenerator only), augmented with
/// social sharing links, SEO metadata, and Schema.org structured data.
/// </summary>
public sealed class PropertyDetailResponse
{
    // ── Basic property information ────────────────────────────────────────
    public byte PropertyId { get; init; }
    public string? DisplayAddress { get; init; }
    public string? Description { get; init; }
    public string? Slug { get; init; }

    // ── Guest capacity ────────────────────────────────────────────────────
    public byte MinimumNumberOfAdult { get; init; }
    public byte MaximumNumberOfGuests { get; init; }
    public byte MaximumNumberOfAdult { get; init; }
    public byte MaximumNumberOfChildren { get; init; }
    public byte MaximumNumberOfInfants { get; init; }

    // ── Room counts ───────────────────────────────────────────────────────
    public byte NumberOfBedrooms { get; init; }
    public double NumberOfBathrooms { get; init; }
    public byte NumberOfReceptionRooms { get; init; }
    public byte NumberOfKitchens { get; init; }
    public byte NumberOfCarSpaces { get; init; }

    // ── Booking rules ─────────────────────────────────────────────────────
    /// <summary>Earliest check-in time (e.g. 15:00).</summary>
    [JsonConverter(typeof(TimeOnlyHHmmConverter))]
    public TimeOnly CheckInTime { get; init; }
    /// <summary>Latest check-out time (e.g. 10:00).</summary>
    [JsonConverter(typeof(TimeOnlyHHmmConverter))]
    public TimeOnly CheckOutTime { get; init; }
    /// <summary>Minimum nights per booking (drives booking form validation).</summary>
    public byte MinimumStayNights { get; init; }
    /// <summary>Maximum nights per booking. Null means no upper limit.</summary>
    public short? MaximumStayNights { get; init; }

    public IReadOnlyList<DateOnly> DatesBooked { get; init; } = [];
    public IReadOnlyList<PropertyFaqResult> Faqs { get; init; } = [];
    public PropertyRatingSummaryResponse? RatingSummary { get; init; }
    public PropertyHostResponse? Host { get; init; }
    public PropertyMapResult? Map { get; init; }
    public IReadOnlyList<string> Amenities { get; init; } = [];
    public IReadOnlyList<PropertyImageResult> Images { get; init; } = [];
    public IReadOnlyList<PropertyBedroomResult> Bedrooms { get; init; } = [];

    // ── Reviews (capped at 10, with formatted relative date display) ──────
    public IReadOnlyList<ReviewResponse> Reviews { get; init; } = [];

    // ── Social sharing ────────────────────────────────────────────────────
    public required PropertyShareLinksResponse ShareLinks { get; init; }

    // ── SEO ───────────────────────────────────────────────────────────────
    public required PropertySeoResult Seo { get; init; }

    /// <summary>Schema.org LodgingBusiness JSON-LD — serialised as a native JSON object (not a string).</summary>
    public required object SchemaOrg { get; init; }

    /// <summary>When this property was last updated. Also returned as the Last-Modified HTTP header.</summary>
    public DateTimeOffset? LastModified { get; init; }

    /// <summary>Embedded video HTML (e.g. YouTube iframe). Omitted when no video is configured.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? VideoHtml { get; init; }

    /// <summary>Property disclaimer text. Omitted when not set.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Disclaimer { get; init; }
}

public sealed class PropertyRatingSummaryResponse
{
    public double Rating { get; init; }
    public int TotalReviews { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Accuracy { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Cleanliness { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Communication { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? CheckInExperience { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Value { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Location { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Facilities { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Comfort { get; init; }

    public static PropertyRatingSummaryResponse FromResult(PropertyRatingSummaryResult result) => new()
    {
        Rating            = Math.Round(result.Rating, 2),
        TotalReviews      = result.TotalReviews,
        Accuracy          = result.Accuracy          is { } acc   ? Math.Round(acc,   2) : null,
        Cleanliness       = result.Cleanliness       is { } cln   ? Math.Round(cln,   2) : null,
        Communication     = result.Communication     is { } com   ? Math.Round(com,   2) : null,
        CheckInExperience = result.CheckInExperience is { } cin   ? Math.Round(cin,   2) : null,
        Value             = result.Value             is { } val   ? Math.Round(val,   2) : null,
        Location          = result.Location          is { } loc   ? Math.Round(loc,   2) : null,
        Facilities        = result.Facilities        is { } fac   ? Math.Round(fac,   2) : null,
        Comfort           = result.Comfort           is { } cmt   ? Math.Round(cmt,   2) : null,
    };
}

/// <summary>
/// API-layer wrapper for review data with presentation formatting
/// </summary>
public sealed class ReviewResponse
{
    public required string Name { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Company { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Position { get; init; }
    public required string Description { get; init; }
    public byte Rating { get; init; }
    public DateOnly? DateAdded { get; init; }
    public required string ReviewType { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LinkToView { get; init; }

    /// <summary>
    /// Human-friendly relative time display (e.g., "2 days ago", "a month ago")
    /// </summary>
    public required string DateToDisplay { get; init; }

    public static ReviewResponse FromApplicationModel(PropertyReviewResult review)
    {
        return new ReviewResponse
        {
            Name = review.Name,
            Company = review.Company,
            Position = review.Position,
            Description = review.Description,
            Rating = review.Rating,
            DateAdded = review.DateTimeAdded.HasValue
                ? DateOnly.FromDateTime(review.DateTimeAdded.Value)
                : null,
            ReviewType = review.ReviewType,
            LinkToView = review.LinkToView,
            DateToDisplay = FormatDateToDisplay(review.DateTimeAdded)
        };
    }

    private static string FormatDateToDisplay(DateTime? dateTimeAdded)
    {
        if (!dateTimeAdded.HasValue)
            return "";
        
        TimeSpan ts = DateTime.Now - dateTimeAdded.Value;

        return ts.Days switch
        {
            0 => ts.Hours switch
            {
                <= 1 => ts.Minutes switch
                {
                    <= 1 => "1 minute ago",
                    < 60 => $"{ts.Minutes} minutes ago",
                    _ => "1 hour ago"
                },
                < 12 => $"{ts.Hours} hours ago",
                _ => "today"
            },
            1 => "1 day ago",
            < 7 => $"{ts.Days} days ago",
            7 => "a week ago",
            > 7 and <= 14 => "2 weeks ago",
            > 14 and <= 21 => "3 weeks ago",
            < 365 => FormatMonthsAgo(dateTimeAdded.Value),
            _ => FormatYearsAgo(ts.Days)
        };
    }

    private static string FormatMonthsAgo(DateTime dateTimeAdded)
    {
        var monthsAgo = (DateTime.Now.Year - dateTimeAdded.Year) * 12 + 
                       DateTime.Now.Month - dateTimeAdded.Month;
        return monthsAgo > 1 ? $"{monthsAgo} months ago" : "a month ago";
    }

    private static string FormatYearsAgo(int days)
    {
        int yearsAgo = days / 365;
        return yearsAgo switch
        {
            1 => "a year ago",
            > 1 => $"{yearsAgo} years ago",
            _ => ""
        };
    }
}

public sealed class PropertyShareLinksResponse
{
    public required string Facebook { get; init; }
    public required string Twitter { get; init; }
    public required string LinkedIn { get; init; }
    public required string Email { get; init; }
}

public sealed class PropertyHostResponse
{
    public required string Name { get; init; }
    public required string JobTitle { get; init; }
    public byte NumberOfProperties { get; init; }
    public byte YearsExperience { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProfileBio { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Location { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ImagePath { get; init; }
}
