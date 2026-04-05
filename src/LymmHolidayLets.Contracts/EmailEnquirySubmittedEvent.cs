namespace LymmHolidayLets.Contracts;

/// <summary>
/// Published when a website contact form enquiry has been saved to the database.
/// Consumed by the EmailWorker to send the enquiry notification to the company inbox.
/// </summary>
public sealed record EmailEnquirySubmittedEvent(
    string Name,
    string? Company,
    string? EmailAddress,
    string? TelephoneNo,
    string? Subject,
    string Message,
    DateTime SubmittedAt);
