using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Slideshow.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class SlideshowQuery : ISlideshowQuery
    {
        private IDapperSlideshowRepository _slideshowRepository;

        public SlideshowQuery(IDapperSlideshowRepository slideshowRepository)
        {
            _slideshowRepository = slideshowRepository;
        }

        public Slideshow? GetById(byte id)
        {
            return _slideshowRepository.GetById(id);
        }

        public IEnumerable<Slideshow> GetAll()
        {
            return _slideshowRepository.GetAll();
        }
    }
}