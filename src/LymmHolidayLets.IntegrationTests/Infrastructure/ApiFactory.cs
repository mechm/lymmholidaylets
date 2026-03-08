using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace LymmHolidayLets.IntegrationTests.Infrastructure;

/// <summary>
/// WebApplicationFactory that replaces all data-layer and external service dependencies with mocks,
/// allowing integration tests to verify routing, middleware, model binding, and response formats
/// without requiring a real database or external services.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    public readonly Mock<IHomepageService> HomepageService = new();
    public readonly Mock<IReviewQuery> ReviewQuery = new();
    public readonly Mock<IPageQuery> PageQuery = new();
    public readonly Mock<ICalService> CalService = new();
    public readonly Mock<ICheckoutService> CheckoutService = new();
    public readonly Mock<IManageCheckoutSessionService> SessionService = new();
    public readonly Mock<IEmailEnquiryService> EmailEnquiryService = new();
    public readonly Mock<IRecaptchaValidationService> RecaptchaValidationService = new();
    public readonly Mock<IStripeWebhookProcessor> StripeWebhookProcessor = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace all application services with mocks so no DB or external calls are made
            services.RemoveAll<IHomepageService>();
            services.AddSingleton(HomepageService.Object);

            services.RemoveAll<IReviewQuery>();
            services.AddSingleton(ReviewQuery.Object);

            services.RemoveAll<IPageQuery>();
            services.AddSingleton(PageQuery.Object);

            services.RemoveAll<ICalService>();
            services.AddSingleton(CalService.Object);

            services.RemoveAll<ICheckoutService>();
            services.AddSingleton(CheckoutService.Object);

            services.RemoveAll<IManageCheckoutSessionService>();
            services.AddSingleton(SessionService.Object);

            services.RemoveAll<IEmailEnquiryService>();
            services.AddSingleton(EmailEnquiryService.Object);

            services.RemoveAll<IRecaptchaValidationService>();
            services.AddSingleton(RecaptchaValidationService.Object);

            services.RemoveAll<IStripeWebhookProcessor>();
            services.AddSingleton(StripeWebhookProcessor.Object);
        });
    }
}
