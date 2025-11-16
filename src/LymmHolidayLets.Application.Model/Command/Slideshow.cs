
namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class Slideshow
    {
        // create
        public Slideshow(string imagePath, string imagePathAlt, string? captionTitle, string? caption,
            string? shortMobileCaption, string? link, byte sequenceOrder, bool visible)
        {
            ImagePath = imagePath;
            ImagePathAlt = imagePathAlt;
            CaptionTitle = captionTitle;
            Caption = caption;
            ShortMobileCaption = shortMobileCaption;
            Link = link;
            SequenceOrder = sequenceOrder;
            Visible = visible;
        }

        // read and update
        public Slideshow(byte slideshowId, string imagePath, string imagePathAlt, string? captionTitle, string? caption,
            string? shortMobileCaption, string? link, byte sequenceOrder, bool visible):this(imagePath, imagePathAlt, captionTitle, caption,
            shortMobileCaption, link, sequenceOrder, visible)
        {
            SlideshowId = slideshowId;
        }

        public byte SlideshowId { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathAlt { get; set; }
        public string? CaptionTitle { get; set; }
        public string? Caption { get; set; }
        public string? ShortMobileCaption { get; set; }
        public string? Link { get; set; }
        public byte SequenceOrder { get; set; }
        public bool Visible { get; set; }
    }
}
