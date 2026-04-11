using LymmHolidayLets.Domain.ReadModel.Review;

namespace LymmHolidayLets.Application.Interface.Service;

public interface IReviewSummaryQueryService
{
    Task<IReadOnlyList<ReviewSummary>> GetApprovedReviewsAsync(CancellationToken cancellationToken = default);
}
