namespace LymmHolidayLets.Domain.ReadModel.Calendar
{
    public sealed class Calendar
    {
        public DateOnly Date { get; set; }
        public int MinimumStay { get; set; }
        public int? MaximumStay { get; set; }
    }
}
