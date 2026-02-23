using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LymmHolidayLets.Domain.Model.Page.Entity
{
    [Table("Page")]
    public sealed class PageEF
    {
        public PageEF() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte PageId { get; set; } // DB: TINYINT IDENTITY -> use byte

        [Required, MaxLength(255)]
        public string AliasTitle { get; set; } = null!;

        [Required, MaxLength(1000)]
        public string MetaDescription { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Title { get; set; } = null!;

        [MaxLength(255)]
        public string? MainImage { get; set; }

        [MaxLength(255)]
        public string? MainImageAlt { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        // Keep template id on the EF entity so queries can return it.
        public byte TemplateId { get; set; }

        [NotMapped]
        public string? TemplateDescription { get; set; } // not stored on Pages table

        public bool Visible { get; set; }
    }
}
