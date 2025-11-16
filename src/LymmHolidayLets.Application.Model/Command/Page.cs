namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class Page
    {
        public byte PageId { get; set; }
        public required string AliasTitle { get; set; }
        public required string MetaDescription { get; set; }
        public required string Title { get; set; }
        public string? MainImage { get; set; }
        public string? MainImageAlt { get; set; }
        public required string Description { get; set; }
        public byte TemplateId { get; set; }
        public bool Visible { get; set; }
    }
}
