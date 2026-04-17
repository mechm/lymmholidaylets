using Microsoft.Extensions.Options;
using System.Web;

namespace LymmHolidayLets.Api.Services;

/// <summary>
/// Generates social media sharing links for property detail pages
/// </summary>
public interface ISocialShareLinkGenerator
{
    SocialShareLinks GenerateLinks(byte propertyId, string? displayAddress, string? slug = null);
}

public sealed class SocialShareLinkGenerator(IOptions<AppSettings> appSettings) : ISocialShareLinkGenerator
{
    private readonly string _frontendBaseUrl = appSettings.Value.Keys?.SiteMaps ?? "https://lymmholidaylets.com";

    public SocialShareLinks GenerateLinks(byte propertyId, string? displayAddress, string? slug = null)
    {
        var propertyPath  = !string.IsNullOrWhiteSpace(slug) ? slug : propertyId.ToString();
        var propertyUrl   = $"{_frontendBaseUrl}/property/{propertyPath}";
        var encodedUrl    = HttpUtility.UrlEncode(propertyUrl);
        var encodedTitle  = HttpUtility.UrlEncode(displayAddress ?? "Property from Lymm Holiday Lets");

        return new SocialShareLinks
        {
            PropertyUrl       = propertyUrl,
            FacebookShareLink = $"https://facebook.com/sharer/sharer.php?u={encodedUrl}",
            TwitterShareLink  = $"https://twitter.com/share?text={encodedTitle}&url={encodedUrl}",
            LinkedInShareLink = $"https://linkedin.com/shareArticle?mini=true&url={encodedUrl}&title={encodedTitle}&source=lymmholidaylets.com",
            EmailShareLink    = $"mailto:?subject=Property%20on%20Lymm%20Holiday%20Lets&body=Saw%20this%20property%20on%20Lymm%20Holiday%20Lets%20and%20thought%20you%20might%20be%20interested.%0A%0AClick%20this%20link%20to%20view%20the%20advert%3A%0A{propertyUrl}"
        };
    }
}

public sealed class SocialShareLinks
{
    public required string PropertyUrl { get; init; }
    public required string FacebookShareLink { get; init; }
    public required string TwitterShareLink { get; init; }
    public required string LinkedInShareLink { get; init; }
    public required string EmailShareLink { get; init; }
}

/// <summary>
/// Application settings model for social sharing configuration
/// </summary>
public class AppSettings
{
    public KeysSettings? Keys { get; set; }
}

public class KeysSettings
{
    public string? SiteMaps { get; set; }
}
