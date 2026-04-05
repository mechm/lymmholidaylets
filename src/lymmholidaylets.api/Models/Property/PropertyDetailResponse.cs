using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Api.Models.Property;

/// <summary>
/// API response wrapper for property details with computed presentation properties.
/// Extends the application-layer PropertyDetailResult with UI-specific social sharing links
/// and formatted review dates.
/// </summary>
public sealed class PropertyDetailResponse
{
    /// <summary>
    /// Core property detail data from the application layer
    /// </summary>
    public required PropertyDetailResult PropertyDetail { get; init; }

    /// <summary>
    /// Computed property URL for the current request
    /// </summary>
    public required string PropertyUrl { get; init; }

    /// <summary>
    /// Facebook share link for this property
    /// </summary>
    public required string FacebookShareLink { get; init; }

    /// <summary>
    /// Twitter share link for this property
    /// </summary>
    public required string TwitterShareLink { get; init; }

    /// <summary>
    /// LinkedIn share link for this property
    /// </summary>
    public required string LinkedInShareLink { get; init; }

    /// <summary>
    /// Email share link for this property
    /// </summary>
    public required string EmailShareLink { get; init; }

    /// <summary>
    /// Page title for SEO/meta tags
    /// </summary>
    public string PageTitle => "Property from Lymm Holiday Lets";

    /// <summary>
    /// Reviews with formatted date display for presentation
    /// </summary>
    public IReadOnlyList<ReviewResponse>? Reviews { get; init; }
}

/// <summary>
/// API-layer wrapper for review data with presentation formatting
/// </summary>
public sealed class ReviewResponse
{
    public required string Name { get; init; }
    public string? Company { get; init; }
    public string? Position { get; init; }
    public required string Description { get; init; }
    public byte Rating { get; init; }
    public DateTime? DateTimeAdded { get; init; }
    public required string ReviewType { get; init; }
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
            DateTimeAdded = review.DateTimeAdded,
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
