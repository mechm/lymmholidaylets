namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyMap
    {
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
