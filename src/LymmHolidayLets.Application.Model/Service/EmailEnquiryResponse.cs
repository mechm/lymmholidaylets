namespace LymmHolidayLets.Application.Model.Service;

public sealed class EmailEnquiryResponse
{
    private EmailEnquiryResponse(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    public static EmailEnquiryResponse Success() => new(true, null);

    public static EmailEnquiryResponse Failure(string errorMessage) => new(false, errorMessage);
}
