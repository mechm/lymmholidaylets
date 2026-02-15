using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LymmHolidayLets.Domain.Model.Property.Entity
{
    [Table("Property")]
    public sealed class PropertyEF
    {
        public PropertyEF() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte ID { get; set; }

        public string FriendlyName { get; set; } = null!;

        // A few commonly used columns; add more as needed.
        public string? DisplayAddress { get; set; }
        public string? Description { get; set; }
        public decimal DefaultNightlyPrice { get; set; }
        public bool? ShowOnHomepage { get; set; }
        public bool? ShowOnSite { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
