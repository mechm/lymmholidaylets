namespace LymmHolidayLets.Api.Services
{
    public interface IRecaptchaValidationService
    {
        Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken = default);
    }
}
