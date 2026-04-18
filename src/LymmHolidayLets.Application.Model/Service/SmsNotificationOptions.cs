namespace LymmHolidayLets.Application.Model.Service
{
    public sealed class SmsNotificationOptions
    {
        public bool Enabled { get; init; } = true;

        public string[] Recipients { get; init; } = [];
    }
}
