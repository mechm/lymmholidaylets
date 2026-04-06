namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyImage
    {
        public required string ImagePath { get; init; }
        public string? AltText { get; init; }
        public byte SequenceOrder { get; init; }
    }
}
