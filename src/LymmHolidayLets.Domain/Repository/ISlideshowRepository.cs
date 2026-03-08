using LymmHolidayLets.Domain.Model.Slideshow.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface ISlideshowRepository : IRepository<Slideshow>
    {
        Slideshow? GetById(byte id);
        IEnumerable<Slideshow> GetAll();
        void Create(Slideshow slideShow);
        void Update(Slideshow slideShow);
        void Delete(byte id);
    }
}
