namespace LymmHolidayLets.Domain.ReadModel.Homepage
{
    public sealed class Review
    {
        public Review(string friendlyName, string? company, string description, string name, string? position, DateTime dateTimeAdded) 
        { 
            FriendlyName = friendlyName;
            Company = company;
            Description = description;
            Name = name;
            Position = position;
            DateTimeAdded = dateTimeAdded;
        }

        public required string FriendlyName { get; set; }
        public string? Company { get; set; }
        public required string Description { get; set; }
        public required string Name { get; set; }      
        public string? Position { get; set; }
        public required DateTime DateTimeAdded { get; set; }
    }
}
