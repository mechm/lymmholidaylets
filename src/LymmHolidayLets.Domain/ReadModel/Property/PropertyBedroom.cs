namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyBedroom
    {
        public byte BedroomNumber { get; init; }
        public string? BedroomName { get; init; }
        public required string BedType { get; init; }
        public string? BedTypeIcon { get; init; }
        public byte NumberOfBeds { get; init; }
    }
}
