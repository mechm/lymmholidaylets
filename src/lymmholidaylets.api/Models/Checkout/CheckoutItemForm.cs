namespace LymmHolidayLets.Api.Models.Checkout
{
    public sealed class 
        CheckoutItemForm
    {
        public byte PropertyId { get; set; }

        public DateOnly Checkin { get; set; }

        public DateOnly Checkout { get; set; }

        public short NumberOfAdults { get; set; }

        public short NumberOfChildren { get; set; }

        public short NumberOfInfants { get; set; }
    }
}
