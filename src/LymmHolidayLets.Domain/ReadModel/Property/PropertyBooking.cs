namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyBooking
    {
        public byte MinimumNumberOfAdult { get; init; }
        public byte MaximumNumberOfGuests { get; init; }
        public byte MaximumNumberOfAdult { get; init; }
        public byte MaximumNumberOfChildren { get; init; }
        public byte MaximumNumberOfInfants { get; init; }
    }
}
