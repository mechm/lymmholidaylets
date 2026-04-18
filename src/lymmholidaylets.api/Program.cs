using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using LymmHolidayLets.Api.Infrastructure.ExceptionHandling;
using Serilog;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Asp.Versioning;
using LymmHolidayLets.Api.GraphQL;
using LymmHolidayLets.Api.Validators;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Infrastructure;
using LymmHolidayLets.Infrastructure.DataAdapter;
using LymmHolidayLets.Infrastructure.DependencyInjection;
using LymmHolidayLets.Infrastructure.Repository;
using LymmHolidayLets.Infrastructure.Repository.Dapper;
using LymmHolidayLets.Infrastructure.Repository.EF;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Command;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using LymmHolidayLets.Application.Service;
using MassTransit;
using Microsoft.OpenApi;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{

Log.Information("Starting LymmHolidayLets.Api");

var builder = WebApplication.CreateBuilder(args);

// Configure application settings for DI
builder.Services.Configure<LymmHolidayLets.Api.Services.AppSettings>(
    builder.Configuration);

// Wire Serilog as the ILogger<T> provider for the entire app.
// Configuration (sinks, enrichers, minimum levels) comes from appsettings.json.
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<EmailEnquiryRequestValidator>();
builder.Services.AddHttpClient();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Lymm Holiday Lets API",
            Version = "v1",
            Description = "API for managing Lymm Holiday Lets bookings, payments, and content."
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? ["https://lymmholidaylets.com"];
    options.AddPolicy("NextJsApp", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ContactForm", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5; // Max 5 requests per minute
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Protect the checkout endpoint from availability probing and abuse.
    // Bots may attempt many date combinations to determine vacancy windows,
    // or repeatedly create sessions to tie up inventory checks.
    // The limit is per client IP — different users are unaffected by each other.
    // 10 per minute is generous for any legitimate user (who would realistically
    // only submit 1-2 times per visit) while still blocking automated scanners.
    options.AddFixedWindowLimiter("CheckoutSession", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiGraphQl();

builder.Services.AddTransient<ICalendarQuery, CalendarQuery>();
builder.Services.AddTransient<ICalQuery, CalQuery>();
builder.Services.AddTransient<IPageQuery, PageQuery>();
builder.Services.AddTransient<IHomepageQuery, HomepageQuery>();
builder.Services.AddTransient<IReviewQuery, ReviewQuery>();
builder.Services.AddTransient<IPropertyQuery, PropertyQuery>();
builder.Services.AddTransient<ISiteMapQuery, SiteMapQuery>();
builder.Services.AddTransient<ICheckoutQuery, CheckoutQuery>();
builder.Services.AddTransient<IWebhookEventQuery, WebhookEventQuery>();


// Data access
builder.Services.AddTransient<IDatabaseFactory, DatabaseFactory>();

// Infrastructure -- DataAdapter
builder.Services.AddTransient<IDapperCheckoutDataAdapter, DapperCheckoutDataAdapter>();
builder.Services.AddTransient<IDapperICalDataAdapter, DapperICalDataAdapter>();
builder.Services.AddTransient<IDapperCalendarDataAdapter, DapperCalendarDataAdapter>();
builder.Services.AddTransient<IDapperPropertyDataAdapter, DapperPropertyDataAdapter>();
builder.Services.AddTransient<IDapperPriceDataAdapter, DapperPriceDataAdapter>();
builder.Services.AddTransient<IDapperPageDataAdapter, DapperPageDataAdapter>();
builder.Services.AddTransient<IDapperTemplateDataAdapter, DapperTemplateDataAdapter>();
builder.Services.AddTransient<IDapperUrlRedirectDataAdapter, DapperUrlRedirectDataAdapter>();
builder.Services.AddTransient<IDapperSiteMapDataAdapter, DapperSiteMapDataAdapter>();
builder.Services.AddTransient<IDapperHomepageDataAdapter, DapperHomepageDataAdapter>();
builder.Services.AddTransient<IDapperReviewDataAdapter, DapperReviewDataAdapter>();


// Infrastructure -- Repository
builder.Services.AddTransient<DbSession>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICheckoutRepository, DapperCheckoutRepository>();
builder.Services.AddTransient<IBookingRepository, DapperBookingRepository>();
builder.Services.AddTransient<ICalendarRepository, DapperCalendarRepository>();
builder.Services.AddTransient<IICalRepository, DapperICalRepository>();
builder.Services.AddTransient<IPageRepository, DapperPageRepository>();
builder.Services.AddTransient<ITemplateRepository, DapperTemplateRepository>();
builder.Services.AddTransient<ISiteMapRepository, DapperSiteMapRepository>();
builder.Services.AddTransient<ISlideshowRepository, DapperSlideshowRepository>();
builder.Services.AddTransient<IEmailEnquiryRepository, DapperEmailEnquiryRepository>();
builder.Services.AddTransient<IFAQRepository, DapperFAQRepository>();
builder.Services.AddTransient<IReviewRepository, DapperReviewRepository>();
builder.Services.AddTransient<IStaffRepository, DapperStaffRepository>();
builder.Services.AddTransient<IWebhookEventRepository, DapperWebhookEventRepository>();


builder.Services.AddTransient<ICalendarRepositoryEF, CalendarRepositoryEF>();
builder.Services.AddTransient<IPropertyRepositoryEF, PropertyRepositoryEF>();
builder.Services.AddTransient<IPageRepositoryEF, PageRepositoryEF>();


// Infrastructure -- Utilities
// IEmailService, IEmailTemplateBuilder and IEmailGeneratorService are registered in the
// EmailWorker project only — the API publishes events rather than sending email directly.
builder.Services.AddTransient<IEmailEnquiryCommand, EmailEnquiryCommand>();
builder.Services.AddTransient<IBookingCommand, BookingCommand>();
builder.Services.AddTransient<IWebhookEventCommand, WebhookEventCommand>();
builder.Services.AddTransient<ICheckoutCommand, CheckoutCommand>();
builder.Services.AddTransient<IReviewCommand, ReviewCommand>();
builder.Services.AddTransient<IFAQCommand, FAQCommand>();




// Add our new services
builder.Services.AddTransient<IStripeService, LymmHolidayLets.Infrastructure.Services.StripeService>();
builder.Services.AddTransient<IStripeWebhookEventParser, LymmHolidayLets.Infrastructure.Services.StripeWebhookEventParser>();
builder.Services.AddTransient<ICalGenerator, CalGenerator>();
builder.Services.AddTransient<ICheckoutService, CheckoutService>();
builder.Services.AddTransient<IHomepageQueryService, HomepageQueryService>();
builder.Services.AddTransient<IPageQueryService, PageQueryService>();
builder.Services.AddTransient<IPropertyDetailQueryService, PropertyDetailQueryService>();
builder.Services.AddTransient<IReviewSummaryQueryService, ReviewSummaryQueryService>();
builder.Services.AddTransient<IEmailEnquiryProcessingService, EmailEnquiryProcessingService>();
builder.Services.AddTransient<ICalendarFeedService, CalendarFeedService>();
builder.Services.AddTransient<IRecaptchaValidationService, LymmHolidayLets.Infrastructure.Services.RecaptchaValidationService>();
builder.Services.AddSingleton<IApplicationCache, LymmHolidayLets.Infrastructure.Services.ApplicationMemoryCache>();
builder.Services.AddTransient<LymmHolidayLets.Api.Services.ISocialShareLinkGenerator, LymmHolidayLets.Api.Services.SocialShareLinkGenerator>();
builder.Services.AddTransient<LymmHolidayLets.Api.Services.IImageUrlResolver, LymmHolidayLets.Api.Services.ImageUrlResolver>();
builder.Services.AddTransient<LymmHolidayLets.Api.Services.ISeoMetaGenerator, LymmHolidayLets.Api.Services.SeoMetaGenerator>();
builder.Services.AddTransient<LymmHolidayLets.Api.Services.ISchemaOrgGenerator, LymmHolidayLets.Api.Services.SchemaOrgGenerator>();
builder.Services.AddTransient<LymmHolidayLets.Api.Services.IPropertyDetailResponseBuilder, LymmHolidayLets.Api.Services.PropertyDetailResponseBuilder>();
builder.Services.AddSingleton<IPropertyCacheInvalidator, LymmHolidayLets.Infrastructure.Services.PropertyCacheInvalidator>();
builder.Services.AddTransient<IStripeWebhookProcessor, StripeWebhookProcessor>();
builder.Services.AddTransient<IManageCheckoutSessionService, ManageCheckoutSessionService>();

// MassTransit — API publishes events only; consumers live in the EmailWorker
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });
    });
});

builder.Services.Configure<LymmHolidayLets.Application.Model.Service.CheckoutOptions>(
    builder.Configuration.GetSection("Checkout"));

// register EF Core with SQL logging (development)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LymmHolidayLets"), sqlOptions =>
    {
        // Enable transient fault handling (retry on failure)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);

        sqlOptions.CommandTimeout(60);
    })
           // Log SQL commands (filter to database command category to reduce noise)
           .LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], 
                                  LogLevel.Information)
           // Show parameter values in logs (ONLY for development / debugging)
           .EnableSensitiveDataLogging()
           // Provide more detailed errors for debugging
           .EnableDetailedErrors());

builder.Services.AddMemoryCache();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("Database")
    .AddCheck("Self", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Configure the HTTP request pipeline.
// http://localhost:5026/scalar/v1
// http://localhost:5026/openapi/v1.json
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

// Serilog HTTP request logging (replaces default ASP.NET Core request logging)
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseCors("NextJsApp");

app.UseRateLimiter();

app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    AllowCachingResponses = false
});

app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();

}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "LymmHolidayLets.Api terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Allows WebApplicationFactory<Program> to reference this entry point in integration tests
public partial class Program { }
