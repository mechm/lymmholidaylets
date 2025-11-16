namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class FAQ
    {
        public byte ID { get; set; }
        public byte PropertyID { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public bool Visible { get; set; }
    }
}