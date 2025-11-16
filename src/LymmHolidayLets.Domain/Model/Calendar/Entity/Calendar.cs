using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.Calendar.Entity
{
	public sealed class Calendar : IAggregateRoot
	{
		// create
		public Calendar(byte propertyID, DateTime date, decimal? price, byte minimumStay, byte? maximumStay, bool available, bool booked, int? bookingID)
		{
			PropertyID = propertyID;
			Date = date;
			Price = price;
			MinimumStay = minimumStay;
			MaximumStay = maximumStay;
			Available = available;
			Booked = booked;
			BookingID = bookingID;
		}

		// update
		public Calendar(int id, byte propertyID, DateTime date, decimal? price, byte minimumStay, byte? maximumStay, bool available, bool booked, int? bookingID)
		{
			ID = id;
			PropertyID = propertyID;
			Date = date;
			Price = price;
			MinimumStay = minimumStay;
			MaximumStay = maximumStay;
			Available = available;
			Booked = booked;
			BookingID = bookingID;
		}

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
