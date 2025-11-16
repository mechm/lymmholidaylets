namespace LymmHolidayLets.Domain.ReadModel.Review
{
    public sealed class ReviewSummary
    {
        public required byte PropertyId { get; set; }
        public required string PropertyName { get; set; }
        public required string Name { get; set; }
        public string? Position { get; set; }
        public string? Company { get; set; }
        public byte Rating { get; set; }
        public required string ReviewType { get; set; }
        public string? LinkToView { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public required string Description { get; set; }

        public string DateToDisplay()
        {
            TimeSpan ts = DateTime.Now - DateTimeAdded;

            if (ts.Days == 0)
            {
                if (ts.Hours <= 1)
                {
                    if (ts.Minutes <= 1)
                    {
                        return "1 minute ago";
                    }

                    if (ts.Minutes < 60)
                    {
                        return $"{ts.Minutes} minutes ago";
                    }

                    return "1 hour ago";
                }

                if (ts.Hours < 12)
                {
                    return $"{ts.Hours} hours ago";
                }

                return "today";
            }
            else if (ts.Days == 1)
            {
                return "1 day ago";
            }
            else if (ts.Days < 7)
            {
                return $"{ts.Days} days ago";
            }
            else if (ts.Days == 7)
            {
                return "a week ago";
            }
            else if (ts.Days < 365)
            {
                if (ts.Days > 7 && ts.Days <= 14)
                {
                    return "2 weeks ago";
                }

                if (ts.Days > 14 && ts.Days <= 21)
                {
                    return "3 weeks ago";
                }

                int monthsAgo = ((DateTime.Now.Year - DateTimeAdded.Year) * 12) + DateTime.Now.Month - DateTimeAdded.Month;

                if (monthsAgo > 1)
                {
                    return $"{monthsAgo} months ago";
                }
                else if (monthsAgo <= 1)
                {
                    return "a month ago";
                }
            }
            else if (ts.Days >= 365)
            {
                int yearsAgo = ts.Days / 365;

                if (yearsAgo == 1)
                {
                    return "a year ago";
                }
                else if (yearsAgo > 1)
                {
                    return $"{yearsAgo} years ago";
                }
            }

            return "";
        }
    }
}