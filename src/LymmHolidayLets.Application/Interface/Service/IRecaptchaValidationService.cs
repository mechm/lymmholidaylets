namespace LymmHolidayLets.Application.Interface.Service;

public interface IRecaptchaValidationService
{
    Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken = default);
}
