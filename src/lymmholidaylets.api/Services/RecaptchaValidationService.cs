using System.Text.Json;
using System.Text.Json.Serialization;

namespace LymmHolidayLets.Api.Services
{
    public sealed class RecaptchaValidationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        Domain.Interface.ILogger logger) : IRecaptchaValidationService
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
                client.Timeout = TimeSpan.FromSeconds(10); // Set reasonable timeout
                
                var values = new Dictionary<string, string>
                {
                    { "secret", privateKey },
                    { "response", token }
                };

                using var content = new FormUrlEncodedContent(values);
                using var response = await client.PostAsync(siteVerifyApiEndpoint, content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning($"ReCaptcha API returned status: {response.StatusCode}");
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
                        logger.LogInfo($"Unsuccessful recaptcha result - {string.Join(", ", result.ErrorMessage)}", "RecaptchaValidationService");
                        break;
                }

                return result.Success;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("ReCaptcha validation was cancelled");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error with recaptcha verification: {ex.Message}", ex);
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
}
