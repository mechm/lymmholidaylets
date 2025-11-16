using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IReviewCommand
    {
        void Create(Review review);
        void Update(Review review);
        void Delete(int id);
    }
}