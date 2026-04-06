using Microsoft.Extensions.Options;

namespace LymmHolidayLets.Api.Services;

public interface IImageUrlResolver
{
    /// <summary>
    /// Converts a relative image path to an absolute URL.
    /// Returns <c>null</c> when <paramref name="path"/> is null or whitespace.
    /// Passes through paths that are already absolute.
    /// </summary>
    string? Resolve(string? path);
}

public sealed class ImageUrlResolver(IOptions<AppSettings> appSettings) : IImageUrlResolver
{
    private readonly string _baseUrl = appSettings.Value.Keys?.SiteMaps ?? "https://www.lymmholidaylets.co.uk";

    public string? Resolve(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        return path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? path
            : $"{_baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }
}
