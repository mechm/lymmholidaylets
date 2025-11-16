using LymmHolidayLets.Domain.Model.Review.Entity;
using LymmHolidayLets.Domain.ReadModel.Review;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IReviewQuery
    {
        Review? GetById(int id);
        IEnumerable<Review> GetAll();
        IEnumerable<ReviewSummary> GetAllApprovedReviews();
    }
}