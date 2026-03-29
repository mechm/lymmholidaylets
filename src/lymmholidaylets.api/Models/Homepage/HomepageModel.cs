namespace LymmHolidayLets.Api.Models.Homepage;

public sealed class HomepageModel(
    IEnumerable<ReviewModel> reviews,
    IEnumerable<SlideshowModel> slides)
{
    public IEnumerable<ReviewModel> Reviews { get; init; } = reviews;
    public IEnumerable<SlideshowModel> Slides { get; init; } = slides;
}