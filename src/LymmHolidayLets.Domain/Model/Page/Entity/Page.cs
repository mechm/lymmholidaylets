using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.Page.Entity
{
    public sealed class Page : IAggregateRoot
    {
        // create
        public Page(string aliasTitle, string metaDescription, string title,
            string? mainImage, string? mainImageAlt, string description,
            Template template, bool visible)
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

        // update and read
        public Page(byte pageId, string aliasTitle, string metaDescription, string title,
            string? mainImage, string? mainImageAlt, string description,
            Template template, bool visible) : this(aliasTitle, metaDescription, title,
            mainImage, mainImageAlt, description, template, visible)
        {
            PageId = pageId;
        }

        public byte PageId { get; set; }
        public string AliasTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Title { get; set; }
        public string? MainImage { get; set; }
        public string? MainImageAlt { get; set; }
        public string Description { get; set; }
        public Template Template { get; set; }
        public bool Visible { get; set; }
    }
}
