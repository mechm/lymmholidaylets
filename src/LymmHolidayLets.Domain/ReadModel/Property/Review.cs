namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class Review 
    { 
        public required string Name { get; set; }
        public string? Company { get; set; }
        public string? Position { get; set; }
        public required string Description { get; set; }
        public byte Rating { get; set; }
        public byte? Cleanliness { get; set; }
        public byte? Accuracy { get; set; }
        public byte? Communication { get; set; }
        public byte? Location { get; set; }
        public byte? Checkin  { get; set; }
        public byte? Facilities  { get; set; }
        public byte? Comfort { get; set; }
        public byte? Value { get; set; }
        public DateTime? DateTimeAdded { get; set; }
        public required string ReviewType { get; set; }
        public string? LinkToView { get; set; }
    }
}