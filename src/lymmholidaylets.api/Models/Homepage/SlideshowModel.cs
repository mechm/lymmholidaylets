namespace LymmHolidayLets.Api.Models.Homepage;

public class SlideshowModel(
    string imagePath,
    string imagePathAlt,
    string? captionTitle,
    string? caption,
    string? shortMobileCaption,
    string? link)
{
    public string ImagePath { get; set; } = imagePath;
    public string ImagePathAlt { get; set; } = imagePathAlt;
    public string? CaptionTitle { get; set; } = captionTitle;
    public string? Caption { get; set; } = caption;
    public string? ShortMobileCaption { get; set; } = shortMobileCaption;
    public string? Link { get; set; } = link;
}