namespace LymmHolidayLets.Domain.ReadModel.Review
{
    public sealed class ReviewSummary
    {
        public required byte PropertyId { get; init; }
        public required string PropertyName { get; init; }
        public required string Name { get; init; }
        public string? Position { get; init; }
        public string? Company { get; init; }
        public byte Rating { get; init; }
        public required string ReviewType { get; init; }
        public string? LinkToView { get; init; }
        public DateTime DateTimeAdded { get; init; }
        public required string Description { get; init; }

        public string DateToDisplay()
        {
            var ts = DateTime.Now - DateTimeAdded;

            switch (ts.Days)
            {
                case 0 when ts.Hours <= 1:
                {
                    return ts.Minutes switch
                    {
                        <= 1 => "1 minute ago",
                        < 60 => $"{ts.Minutes} minutes ago",
                        _ => "1 hour ago"
                    };
                }
                case 0 when ts.Hours < 12:
                    return $"{ts.Hours} hours ago";
                case 0:
                    return "today";
                case 1:
                    return "1 day ago";
                case < 7:
                    return $"{ts.Days} days ago";
                case 7:
                    return "a week ago";
                case < 365 and > 7 and <= 14:
                    return "2 weeks ago";
                case < 365 and > 14 and <= 21:
                    return "3 weeks ago";
                case < 365:
                {
                    var monthsAgo = ((DateTime.Now.Year - DateTimeAdded.Year) * 12) + DateTime.Now.Month - DateTimeAdded.Month;

                    return monthsAgo switch
                    {
                        > 1 => $"{monthsAgo} months ago",
                        <= 1 => "a month ago"
                    };
                }
                case >= 365:
                {
                    var yearsAgo = ts.Days / 365;

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
}