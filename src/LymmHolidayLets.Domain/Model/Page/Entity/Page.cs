using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.Page.Entity
{
    // create
    public sealed class Page(
        string aliasTitle,
        string metaDescription,
        string title,
        string? mainImage,
        string? mainImageAlt,
        string description,
        Template template,
        bool visible)
        : IAggregateRoot
    {
        
        // update and read
        public Page(byte pageId, string aliasTitle, string metaDescription, string title,
            string? mainImage, string? mainImageAlt, string description,
            Template template, bool visible) : this(aliasTitle, metaDescription, title,
            mainImage, mainImageAlt, description, template, visible)
        {
            PageId = pageId;
        }

        public byte PageId { get; set; }
        public string AliasTitle { get; set; } = aliasTitle;
        public string MetaDescription { get; set; } = metaDescription;
        public string Title { get; set; } = title;
        public string? MainImage { get; set; } = mainImage;
        public string? MainImageAlt { get; set; } = mainImageAlt;
        public string Description { get; set; } = description;
        public Template Template { get; set; } = template;
        public bool Visible { get; set; } = visible;
    }
}
