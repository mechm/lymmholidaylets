namespace LymmHolidayLets.EmailScheduler;

public sealed class GuestPreArrivalEmailSchedulerOptions
{
    public int IntervalMinutes { get; set; } = 30;
    public int ReservationTimeoutMinutes { get; set; } = 15;
}
