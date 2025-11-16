namespace LymmHolidayLets.Application.Model.Dto
{
    public sealed class CalendarDto
    {
        public int ID { get; init; }
        public byte PropertyID { get; init; }
        public DateTime Date { get; init; }
        public decimal? Price { get; init; }
        public byte MinimumStay { get; init; }
        public short? MaximumStay { get; init; }
        public bool Available { get; init; }
        public bool Booked { get; init; }
        public int? BookingID { get; init; }
    }
}
