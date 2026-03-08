using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Review.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.ReadModel.Review;

namespace LymmHolidayLets.Application.Query
{
    public sealed class ReviewQuery(
        IReviewRepository reviewRepository,
        IDapperReviewDataAdapter reviewDataAdapter)
        : IReviewQuery
    {
        public Review? GetById(int id)
        {
            return reviewRepository.GetById(id);
        }

        public IEnumerable<Review> GetAll()
        {
            return reviewRepository.GetAll();
        }

        public IReadOnlyList<ReviewSummary> GetAllApprovedReviews()
        {
            return reviewDataAdapter.GetAllApprovedReviews();
        }

        public Task<IReadOnlyList<ReviewSummary>> GetAllApprovedReviewsAsync()
        {
            return reviewDataAdapter.GetAllApprovedReviewsAsync();
        }
    }
}
