using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LymmHolidayLets.Domain.Model.Calendar.Entity
{
  

    [Table("Calendar")]
    public sealed class CalendarEF
    {
        public CalendarEF() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public byte PropertyID { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
        public byte MinimumStay { get; set; }
        public short? MaximumStay { get; set; }
        public bool Available { get; set; }
        public bool Booked { get; set; }
        public int? BookingID { get; set; }
    }
}
