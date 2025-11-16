namespace LymmHolidayLets.Domain.ReadModel.Page
{
    public sealed class PageSummary
    {
        public PageSummary(int pageId, string aliasTitle, string title, string templateDescription, bool visible)
        {
            PageId = pageId;
            AliasTitle = aliasTitle;
            Title = title;
            TemplateDescription = templateDescription;
            Visible = visible;
        }

        public int PageId { get; set; }
        public string AliasTitle { get; set; }
        public string Title { get; set; }
        public string TemplateDescription { get; set; }
        public bool Visible { get; set; }
    }
}
