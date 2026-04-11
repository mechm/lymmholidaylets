using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Api.Services;
using MassTransit;
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
    public readonly Mock<IHomepageQueryService> HomepageQueryService = new();
    public readonly Mock<IReviewSummaryQueryService> ReviewSummaryQueryService = new();
    public readonly Mock<IPageQueryService> PageQueryService = new();
    public readonly Mock<ICalendarFeedService> CalendarFeedService = new();
    public readonly Mock<ICheckoutService> CheckoutService = new();
    public readonly Mock<IManageCheckoutSessionService> SessionService = new();
    public readonly Mock<IEmailEnquiryProcessingService> EmailEnquiryProcessingService = new();
    public readonly Mock<IStripeWebhookProcessor> StripeWebhookProcessor = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace all application services with mocks so no DB or external calls are made
            services.RemoveAll<IHomepageQueryService>();
            services.AddSingleton(HomepageQueryService.Object);

            services.RemoveAll<IReviewSummaryQueryService>();
            services.AddSingleton(ReviewSummaryQueryService.Object);

            services.RemoveAll<IPageQueryService>();
            services.AddSingleton(PageQueryService.Object);

            services.RemoveAll<ICalendarFeedService>();
            services.AddSingleton(CalendarFeedService.Object);

            services.RemoveAll<ICheckoutService>();
            services.AddSingleton(CheckoutService.Object);

            services.RemoveAll<IManageCheckoutSessionService>();
            services.AddSingleton(SessionService.Object);

            services.RemoveAll<IEmailEnquiryProcessingService>();
            services.AddSingleton(EmailEnquiryProcessingService.Object);

            services.RemoveAll<IStripeWebhookProcessor>();
            services.AddSingleton(StripeWebhookProcessor.Object);

            // Replace RabbitMQ transport with in-memory so no broker is required
            services.RemoveMassTransitHostedService();
            services.AddMassTransitTestHarness();
        });
    }
}
