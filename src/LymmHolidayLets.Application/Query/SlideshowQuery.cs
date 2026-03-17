using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Slideshow.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class SlideshowQuery(ISlideshowRepository slideshowRepository) : ISlideshowQuery
    {
        public Slideshow? GetById(byte id)
        {
            return slideshowRepository.GetById(id);
        }

        public IEnumerable<Slideshow> GetAll()
        {
            return slideshowRepository.GetAll();
        }
    }
}
