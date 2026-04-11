namespace LymmHolidayLets.Application.Model.Service;

public sealed class EmailEnquirySubmission
{
    public required string Name { get; init; }
    public string? Company { get; init; }
    public required string EmailAddress { get; init; }
    public string? TelephoneNo { get; init; }
    public string? Subject { get; init; }
    public required string Message { get; init; }
    public string? ReCaptchaToken { get; init; }
    public string? ClientIp { get; init; }
}
