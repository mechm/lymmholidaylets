namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyBooking
    {
        public byte ID { get; init; }
        public byte MinimumNumberOfAdult { get; init; }
        public byte MaximumNumberOfGuests { get; init; }
        public byte MaximumNumberOfAdult { get; init; }
        public byte MaximumNumberOfChildren { get; init; }
        public byte MaximumNumberOfInfants { get; init; }
        public string? DisplayAddress { get; init; }
        public string? PageDescription { get; init; }
        
        // Host information
        public required string HostName { get; init; }
        public string? HostLocation { get; init; }
        public byte NumberOfProperties { get; init; }
        public byte HostYearsExperience { get; init; }
        public required string HostJobTitle { get; init; }
        public string? HostProfileBio { get; init; }
        public string? HostImagePath { get; init; }
        
        // Map information
        public bool ShowMap { get; init; }
        public bool ShowStreetView { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int MapZoom { get; init; }
        public double StreetViewLatitude { get; init; }
        public double StreetViewLongitude { get; init; }
        public double Pitch { get; init; }
        public double Yaw { get; init; }
        public double Zoom { get; init; }
    }
}
