using LymmHolidayLets.Domain.Model.Slideshow.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface ISlideshowQuery
    {
        Slideshow? GetById(byte id);
        IEnumerable<Slideshow> GetAll();
    }
}