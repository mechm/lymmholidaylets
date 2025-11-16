namespace LymmHolidayLets.Application.Interface.Command
{
    public interface ISlideshowCommand
    {
        void Create(Model.Command.Slideshow slideshow);
        void Update(Model.Command.Slideshow slideshow);
        void Delete(byte id);
    }
}