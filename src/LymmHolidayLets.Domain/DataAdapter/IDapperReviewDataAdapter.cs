using LymmHolidayLets.Domain.ReadModel.Review;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperReviewDataAdapter
    {
        IEnumerable<ReviewSummary> GetAllApprovedReviews();
    }
}
