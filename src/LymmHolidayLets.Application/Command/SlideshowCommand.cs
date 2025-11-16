using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class SlideshowCommand : ISlideshowCommand
    {
        private readonly IDapperSlideshowRepository _slideshowRepository;

        public SlideshowCommand(IDapperSlideshowRepository slideshowRepository)
        {
            _slideshowRepository = slideshowRepository;
        }

        public void Create(Model.Command.Slideshow slideshow)
        {
            _slideshowRepository.Create(new Domain.Model.Slideshow.Entity.Slideshow(slideshow.ImagePath,
                slideshow.ImagePathAlt, slideshow.CaptionTitle,
                slideshow.Caption, slideshow.ShortMobileCaption, slideshow.Link, slideshow.SequenceOrder,
                slideshow.Visible));
        }

        public void Update(Model.Command.Slideshow slideshow)
        {
            _slideshowRepository.Update(new Domain.Model.Slideshow.Entity.Slideshow(slideshow.SlideshowId, slideshow.ImagePath,
                slideshow.ImagePathAlt, slideshow.CaptionTitle,
                slideshow.Caption, slideshow.ShortMobileCaption, slideshow.Link, slideshow.SequenceOrder,
                slideshow.Visible));
        }

        public void Delete(byte id)
        {
            _slideshowRepository.Delete(id);
        }
    }
}