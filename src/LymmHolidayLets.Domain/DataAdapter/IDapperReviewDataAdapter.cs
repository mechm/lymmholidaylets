﻿using LymmHolidayLets.Domain.ReadModel.Review;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperReviewDataAdapter
    {
        IReadOnlyList<ReviewSummary> GetAllApprovedReviews();
        Task<IReadOnlyList<ReviewSummary>> GetAllApprovedReviewsAsync();
    }
}
