namespace LymmHolidayLets.Domain.Model.UrlRedirect.ValueType
{
    public sealed class UrlRedirect
    {
        public required string UrlRedirectTo { get; set; }

        public required string UrlFrom { get; set; }
    }
}
