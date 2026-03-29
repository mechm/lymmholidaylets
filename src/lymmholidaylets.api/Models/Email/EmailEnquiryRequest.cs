namespace LymmHolidayLets.Api.Models.Email;

public class EmailEnquiryRequest
{
    public required string Name { get; init; }

    public string? Company { get; set; }

    public required string EmailAddress { get; init; }

    public string? TelephoneNo { get; set; }

    public string? Subject { get; set; }

    public required string Message { get; init; }

    public string? ReCaptchaToken { get; init; }
}
