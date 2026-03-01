using Microsoft.AspNetCore.Mvc.ModelBinding;
using LymmHolidayLets.Api.Models;

namespace LymmHolidayLets.Api.Infrastructure.ModelBinding
{
    public sealed class StripeWebhookModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var request = bindingContext.HttpContext.Request;

            // 1. Get the Stripe signature header
            if (!request.Headers.TryGetValue("Stripe-Signature", out var signature))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            // 2. Read the raw request body
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(json))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            // 3. Create the model
            var result = new StripeWebhookRequest
            {
                Json = json,
                Signature = signature.ToString()
            };

            bindingContext.Result = ModelBindingResult.Success(result);
        }
    }
}
