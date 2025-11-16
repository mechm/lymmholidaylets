namespace LymmHolidayLets.Domain.ReadModel.AvailabilityICal
{
    public sealed class AvailabilityICal
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? BookingID { get; set; }
        public string? Name { get; set; }
        public short? LastFourDigitTelephone { get; set; }
        public short? NoOfGuests { get; set; }
        public short? NoOfNights { get; set; }
        public required string FriendlyName { get; set; }

    }
}
