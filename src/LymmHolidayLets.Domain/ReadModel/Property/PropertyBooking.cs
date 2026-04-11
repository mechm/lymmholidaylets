using LymmHolidayLets.Domain.Model.Property.ValueObject;

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
        public PropertyOccupancy Occupancy => new(
            MinimumNumberOfAdult,
            MaximumNumberOfGuests,
            MaximumNumberOfAdult,
            MaximumNumberOfChildren,
            MaximumNumberOfInfants);
        public string? DisplayAddress { get; init; }
        public string? Description { get; init; }
        public string? MetaDescription { get; init; }
        public string? Slug { get; init; }
        
        // Room counts
        public byte NumberOfBedrooms { get; init; }
        public double NumberOfBathrooms { get; init; }
        public byte NumberOfReceptionRooms { get; init; }
        public byte NumberOfKitchens { get; init; }
        public byte NumberOfCarSpaces { get; init; }
        
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

        public TimeOnly CheckInTimeAfter { get; init; }
        public TimeOnly CheckOutTimeBefore { get; init; }
        public byte MinimumStayNights { get; init; }
        public short? MaximumStayNights { get; init; }
        public DateTime? Updated { get; init; }
        public DateTime? CalendarLastModified { get; init; }
        public string? VideoHtml { get; init; }
        public string? Disclaimer { get; init; }
    }
}
