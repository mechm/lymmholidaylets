namespace LymmHolidayLets.Domain.ReadModel.Homepage
{
    public sealed class Slideshow
    {   
        public required string ImagePath { get; set; }
        public required string ImagePathAlt { get; set; }
        public string? CaptionTitle { get; set; }
        public string? Caption { get; set; }
        public string? ShortMobileCaption { get; set; }
        public string? Link { get; set; }      
    }
}