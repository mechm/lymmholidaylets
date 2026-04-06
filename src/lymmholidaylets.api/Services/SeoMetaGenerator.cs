using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Api.Services;

public interface ISeoMetaGenerator
{
    PropertySeoResult Generate(PropertyDetailResult detail, string propertyUrl);
}

public sealed class SeoMetaGenerator(IImageUrlResolver imageUrlResolver) : ISeoMetaGenerator
{
    private const string SiteName = "Lymm Holiday Lets";
    private const int MetaDescriptionMaxLength = 160;

    public PropertySeoResult Generate(PropertyDetailResult detail, string propertyUrl)
    {
        var metaTitle       = BuildMetaTitle(detail.DisplayAddress);
        var metaDescription = BuildMetaDescription(detail.MetaDescription, detail.Description);
        var ogImage         = detail.Images.Count > 0 ? imageUrlResolver.Resolve(detail.Images[0].ImagePath) : null;
        var ogImageAlt      = detail.Images.Count > 0 ? detail.Images[0].AltText : null;

        return new PropertySeoResult
        {
            MetaTitle       = metaTitle,
            MetaDescription = metaDescription,
            CanonicalUrl    = propertyUrl,
            OgTitle         = metaTitle,
            OgDescription   = metaDescription,
            OgImage         = ogImage,
            OgImageAlt      = ogImageAlt
        };
    }

    private static string BuildMetaTitle(string? displayAddress) =>
        string.IsNullOrWhiteSpace(displayAddress)
            ? SiteName
            : $"{displayAddress} | {SiteName}";

    private static string BuildMetaDescription(string? metaDescription, string? description)
    {
        if (!string.IsNullOrWhiteSpace(metaDescription))
            return metaDescription;

        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        return description.Length <= MetaDescriptionMaxLength
            ? description
            : TruncateAtWordBoundary(description, MetaDescriptionMaxLength);
    }

    private static string TruncateAtWordBoundary(string text, int maxLength)
    {
        var truncated = text[..maxLength];
        var lastSpace = truncated.LastIndexOf(' ');
        return lastSpace > 0
            ? truncated[..lastSpace] + "..."
            : truncated + "...";
    }
}
