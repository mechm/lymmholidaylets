namespace LymmHolidayLets.Application.Model.Property;

/// <summary>
/// SEO metadata for a property detail page, ready for use in HTML meta tags and OG tags.
/// </summary>
public sealed class PropertySeoResult
{
    public required string MetaTitle { get; init; }
    public required string MetaDescription { get; init; }
    public required string CanonicalUrl { get; init; }
    public required string OgTitle { get; init; }
    public required string OgDescription { get; init; }
    public string? OgImage { get; init; }
    public string? OgImageAlt { get; init; }
}
