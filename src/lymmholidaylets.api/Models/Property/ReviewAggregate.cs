using System.Globalization;

namespace LymmHolidayLets.Api.Models.Property;

public sealed class ReviewAggregate
    {
        public double OverallRating { get; set; }
        public string OverallRatingDisplay => OverallRating % 1 > 0 ? OverallRating.ToString("N2") : OverallRating.ToString(CultureInfo.InvariantCulture);

        public double? OverallCleanliness { get; set; }
        public double? OverallAccuracy { get; set; }
        public double? OverallCommunication { get; set; }
        public double? OverallCheckIn { get; set; }
        public double? OverallLocation { get; set; }
        public double? OverallFacilities { get; set; }
        public double? OverallComfort { get; set; }
        public double? OverallValue { get; set; }

        public static string GetPercentage(double value)
        {
            return (value / 5f * 100).ToString(CultureInfo.InvariantCulture) + "%";
        }

        public required IEnumerable<Review> Reviews { get; set; }
    }

    public sealed class Review
    {
        public required string Name { get; set; }
        public string? Company { get; set; }
        public string? Position { get; set; }
        public required string Description { get; set; }
        public int Rating { get; set; }
        public DateTime? DateTimeAdded { get; set; }
        public required string ReviewType { get; set; }
        public string? LinkToView { get; set; }

        public string DateToDisplay()
        {
            if (!DateTimeAdded.HasValue)
            {
                return "";
            }
            
            TimeSpan ts = DateTime.Now - DateTimeAdded.Value;

            switch (ts.Days)
            {
                case 0:
                    return ts.Hours switch
                    {
                        <= 1 => ts.Minutes switch
                        {
                            <= 1 => "1 minute ago",
                            < 60 => $"{ts.Minutes} minutes ago",
                            _ => "1 hour ago"
                        },
                        < 12 => $"{ts.Hours} hours ago",
                        _ => "today"
                    };
                case 1:
                    return "1 day ago";
                case < 7:
                    return $"{ts.Days} days ago";
                case 7:
                    return "a week ago";
                case < 365:
                    switch (ts.Days)
                    {
                        case > 7 and <= 14:
                            return "2 weeks ago";
                        case > 14 and <= 21:
                            return "3 weeks ago";
                        default:
                        {
                            var monthsAgo = (DateTime.Now.Year - DateTimeAdded.Value.Year) * 12 + DateTime.Now.Month - DateTimeAdded.Value.Month;

                            return monthsAgo > 1 ? $"{monthsAgo} months ago" : "a month ago";
                        }
                    }

                    break;
                case >= 365:
                {
                    int yearsAgo = ts.Days / 365;

                    switch (yearsAgo)
                    {
                        case 1:
                            return "a year ago";
                        case > 1:
                            return $"{yearsAgo} years ago";
                    }

                    break;
                }
            }

            return "";
        }
    }