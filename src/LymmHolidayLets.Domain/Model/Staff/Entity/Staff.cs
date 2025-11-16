using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.Staff.Entity
{
    public sealed class Staff : IAggregateRoot
    {
        // create
        public Staff(string name, byte yearsExperience, string jobTitle, string? profileBio,
            string? linkedInLink, string imagePath, bool visible) 
        {
            Name = name;
            YearsExperience = yearsExperience;
            JobTitle = jobTitle;
            ProfileBio = profileBio;
            LinkedInLink = linkedInLink;
            ImagePath = imagePath;
            Visible = visible;
        }

        // update, read
        public Staff(byte id, string name, byte yearsExperience, string jobTitle, string? profileBio,
            string? linkedInLink, string imagePath, bool visible):this(name,yearsExperience, jobTitle, profileBio,
            linkedInLink, imagePath, visible)
        {
            ID = id;
        }

        public byte ID { get; set; }
        public string Name { get; set; }
        public byte YearsExperience { get; set; }
        public string JobTitle { get; set; }
        public string? ProfileBio { get; set; }
        public string? LinkedInLink { get; set; }
        public string ImagePath { get; set; }
        public bool Visible { get; set; }
    }
}
