namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class SiteMap
    {
        public byte SiteMapId { get; set; }
        public required string Url { get; set; }
    }
}
