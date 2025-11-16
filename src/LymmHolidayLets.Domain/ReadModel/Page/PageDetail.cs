namespace LymmHolidayLets.Domain.ReadModel.Page
{
    public sealed class PageDetail
    {
        public PageDetail(string aliasTitle, string metaDescription, string title,
            string mainImage, string mainImageAlt, string description,
            string template, bool visible)
        {
            AliasTitle = aliasTitle;
            MetaDescription = metaDescription;
            Title = title;
            MainImage = mainImage;
            MainImageAlt = mainImageAlt;
            Description = description;
            Template = template;
            Visible = visible;
        }

        public string AliasTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string MainImageAlt { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public bool Visible { get; set; }
    }
}
