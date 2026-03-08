namespace LymmHolidayLets.Domain.ReadModel.Page
{
    public sealed class PageDetail(
        string aliasTitle,
        string metaDescription,
        string title,
        string mainImage,
        string mainImageAlt,
        string description,
        string template,
        bool visible)
    {
        public string AliasTitle { get; } = aliasTitle;
        public string MetaDescription { get; set; } = metaDescription;
        public string Title { get; set; } = title;
        public string MainImage { get; set; } = mainImage;
        public string MainImageAlt { get; set; } = mainImageAlt;
        public string Description { get; set; } = description;
        public string Template { get; set; } = template;
        public bool Visible { get; set; } = visible;
    }
}
