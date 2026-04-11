namespace LymmHolidayLets.Application.Model.Service;

public sealed class CalendarFeedResult
{
    public required byte[] FileContents { get; init; }
    public required string ContentType { get; init; }
    public required string FileDownloadName { get; init; }
}
