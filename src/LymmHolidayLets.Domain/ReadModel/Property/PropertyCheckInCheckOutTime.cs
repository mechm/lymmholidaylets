namespace LymmHolidayLets.Domain.ReadModel.Property
{
    public sealed class PropertyCheckInCheckOutTime
    { 
        public TimeOnly CheckInTimeAfter { get; set; }
        public TimeOnly CheckOutTimeBefore { get; set; }
    }
}