namespace LymmHolidayLets.Domain.Model.Review.ValueObject
{
    public sealed record ReviewRatings(
        byte Overall,
        byte? Accuracy,
        byte? Cleanliness,
        byte? Communication,
        byte? CheckIn,
        byte? Location,
        byte? Facilities,
        byte? Comfort,
        byte? Value)
    {
        public static ReviewRatingsSummary? Summarize(IEnumerable<ReviewRatings> ratings, int totalReviews)
        {
            var list = ratings.ToList();
            if (list.Count == 0)
            {
                return null;
            }

            return new ReviewRatingsSummary(
                list.Average(r => r.Overall),
                AverageOptional(list, r => r.Accuracy),
                AverageOptional(list, r => r.Cleanliness),
                AverageOptional(list, r => r.Communication),
                AverageOptional(list, r => r.CheckIn),
                AverageOptional(list, r => r.Location),
                AverageOptional(list, r => r.Facilities),
                AverageOptional(list, r => r.Comfort),
                AverageOptional(list, r => r.Value),
                totalReviews);
        }

        private static double? AverageOptional(
            IReadOnlyCollection<ReviewRatings> ratings,
            Func<ReviewRatings, byte?> selector)
        {
            var values = ratings
                .Select(selector)
                .Where(value => value.HasValue)
                .Select(value => (double)value!.Value)
                .ToList();

            return values.Count > 0 ? values.Average() : null;
        }
    }

    public sealed record ReviewRatingsSummary(
        double Overall,
        double? Accuracy,
        double? Cleanliness,
        double? Communication,
        double? CheckIn,
        double? Location,
        double? Facilities,
        double? Comfort,
        double? Value,
        int TotalReviews);
}
