using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class SlideshowCommand(ISlideshowRepository slideshowRepository) : ISlideshowCommand
    {
        public void Create(Model.Command.Slideshow slideshow)
        {
            slideshowRepository.Create(new Domain.Model.Slideshow.Entity.Slideshow(slideshow.ImagePath,
                slideshow.ImagePathAlt, slideshow.CaptionTitle,
                slideshow.Caption, slideshow.ShortMobileCaption, slideshow.Link, slideshow.SequenceOrder,
                slideshow.Visible));
        }

        public void Update(Model.Command.Slideshow slideshow)
        {
            slideshowRepository.Update(new Domain.Model.Slideshow.Entity.Slideshow(slideshow.SlideshowId, slideshow.ImagePath,
                slideshow.ImagePathAlt, slideshow.CaptionTitle,
                slideshow.Caption, slideshow.ShortMobileCaption, slideshow.Link, slideshow.SequenceOrder,
                slideshow.Visible));
        }

        public void Delete(byte id)
        {
            slideshowRepository.Delete(id);
        }
    }
}
