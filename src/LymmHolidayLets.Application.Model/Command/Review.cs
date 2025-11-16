namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class Review
    {
        public byte ReviewId { get; set; }
        public byte PropertyID { get; set; }
        public string? Company { get; set; }
        public required string Description { get; set; }
        public string? PrivateNote { get; set; }
        public required string Name { get; set; }
        public string? EmailAddress { get; set; }
        public string? Position { get; set; }
        public byte Rating { get; set; }
        public byte? Cleanliness { get; set; }
        public byte? Accuracy { get; set; }
        public byte? Communication { get; set; }
        public byte? Location { get; set; }
        public byte? Checkin { get; set; }
        public byte? Facilities { get; set; }
        public byte? Comfort { get; set; }
        public byte? Value { get; set; }
        public byte? ReviewTypeId { get; set; }
        public string? LinkToView { get; set; }
        public bool? ShowOnHomepage { get; set; }
        public DateTime? DateTimeAdded { get; set; }
        public DateTime? DateTimeApproved { get; set; }
        public Guid RegistrationCode { get; set; }
        public bool Approved { get; set; }
        public DateTime Created { get; set; }
    }
}
