using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LymmHolidayLets.Domain.Model.Property.Entity
{
    //TODO finish this class and add more columns as needed. This is just a starting point.
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
