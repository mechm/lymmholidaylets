namespace LymmHolidayLets.Application.Model.Service;

public sealed class HomepageResult(
    IReadOnlyList<HomepageReviewResult> reviews,
    IReadOnlyList<HomepageSlideshowResult> slides)
{
    public IReadOnlyList<HomepageReviewResult> Reviews { get; init; } = reviews;
    public IReadOnlyList<HomepageSlideshowResult> Slides { get; init; } = slides;
}

public sealed record HomepageReviewResult(
    string FriendlyName,
    string? Company,
    string Description,
    string Name,
    string? Position,
    DateTime DateTimeAdded);

public sealed record HomepageSlideshowResult(
    string ImagePath,
    string ImagePathAlt,
    string? CaptionTitle,
    string? Caption,
    string? ShortMobileCaption,
    string? Link);
