using System.Text.Json;
using System.Text.Json.Serialization;
using LymmHolidayLets.Application.Interface.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Infrastructure.Services;

public sealed class RecaptchaValidationService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<RecaptchaValidationService> logger) : IRecaptchaValidationService
{
    public async Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        try
        {
            var privateKey = configuration["Keys:RecaptchaPrivateKeyInvisible"];
            var siteVerifyApiEndpoint = configuration["Keys:RecaptchaSiteVerifyApiEndpoint"];

            if (string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(siteVerifyApiEndpoint))
            {
                logger.LogError("Missing RecaptchaPrivateKeyInvisible or RecaptchaSiteVerifyApiEndpoint in configuration.");
                return false;
            }

            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var values = new Dictionary<string, string>
            {
                { "secret", privateKey },
                { "response", token }
            };

            using var content = new FormUrlEncodedContent(values);
            using var response = await client.PostAsync(siteVerifyApiEndpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("ReCaptcha API returned status: {StatusCode}", response.StatusCode);
                return false;
            }

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ReCaptchaJsonResponse>(responseString);

            switch (result)
            {
                case null:
                    logger.LogWarning("ReCaptcha response was null or could not be deserialized");
                    return false;
                case { Success: false, ErrorMessage: not null }:
                    logger.LogInformation("Unsuccessful recaptcha result - {ErrorMessages}", string.Join(", ", result.ErrorMessage));
                    break;
            }

            return result.Success;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("ReCaptcha validation was cancelled");
            return false;
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error with recaptcha verification");
            return false;
        }
    }
}

public sealed class ReCaptchaJsonResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("error-codes")]
    public List<string>? ErrorMessage { get; init; }
}
