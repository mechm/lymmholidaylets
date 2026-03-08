using LymmHolidayLets.Domain.Model.Review.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IReviewRepository : IRepository<Review>
    {
        Review? GetById(int id);
        IEnumerable<Review> GetAll();
        void Create(Review review);
        void Update(Review review);
        void Delete(int id);
    }
}
