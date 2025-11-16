using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Review.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.ReadModel.Review;

namespace LymmHolidayLets.Application.Query
{
    public sealed class ReviewQuery : IReviewQuery
    {
        private readonly IDapperReviewRepository _reviewRepository;
        private readonly IDapperReviewDataAdapter _reviewDataAdapter;

        public ReviewQuery(IDapperReviewRepository reviewRepository,
            IDapperReviewDataAdapter reviewDataAdapter)
        {
            _reviewRepository = reviewRepository;
            _reviewDataAdapter = reviewDataAdapter;
        }

        public Review? GetById(int id)
        {
            return _reviewRepository.GetById(id);
        }

        public IEnumerable<Review> GetAll()
        {
            return _reviewRepository.GetAll();
        }

        public IEnumerable<ReviewSummary> GetAllApprovedReviews()
        {
            return _reviewDataAdapter.GetAllApprovedReviews();
        }
    }
}