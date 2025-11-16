namespace LymmHolidayLets.Domain.ReadModel.Checkout
{
	public sealed class Calendar
	{
		public DateTime Date { get; set; }
		public decimal Price { get; set; }
        public int MinimumStay { get; set; }
		public int? MaximumStay { get; set; }
		public bool Available { get; set; }
    }
}
