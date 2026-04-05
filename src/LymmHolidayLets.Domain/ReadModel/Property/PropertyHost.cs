namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyHost
    {
        public required string HostName { get; init; }

        public byte NumberOfProperties { get; init; }
        public byte HostYearsExperience { get; init; }
        public required string HostJobTitle { get; init; }
        public string? HostProfileBio { get; init; }
        public string? HostImagePath { get; init; }
    }
}
