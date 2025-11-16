namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class Staff
    {
        public byte ID { get; set; }
        public required string Name { get; set; }
        public byte YearsExperience { get; set; }
        public required string JobTitle { get; set; }
        public string? ProfileBio { get; set; }
        public string? LinkedInLink { get; set; }
        public required string ImagePath { get; set; }
        public bool Visible { get; set; }
    }
}
