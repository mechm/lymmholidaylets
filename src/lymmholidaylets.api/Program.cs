using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using LymmHolidayLets.Api.GraphQL;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Infrastructure;
using LymmHolidayLets.Infrastructure.DataAdapter;
using LymmHolidayLets.Infrastructure.DependencyInjection;
using LymmHolidayLets.Infrastructure.Logging;
using LymmHolidayLets.Infrastructure.Repository;
using LymmHolidayLets.Infrastructure.Repository.Dapper;
using LymmHolidayLets.Infrastructure.Repository.EF;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Command;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Infrastructure.Emailer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? ["https://www.lymmholidaylets.co.uk"];
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
builder.Services.AddTransient<IDapperCheckoutRepository, DapperCheckoutRepository>();
builder.Services.AddTransient<IDapperBookingRepository, DapperBookingRepository>();
builder.Services.AddTransient<IDapperCalendarRepository, DapperCalendarRepository>();
builder.Services.AddTransient<IDapperICalRepository, DapperICalRepository>();
builder.Services.AddTransient<IDapperPageRepository, DapperPageRepository>();
builder.Services.AddTransient<IDapperTemplateRepository, DapperTemplateRepository>();
builder.Services.AddTransient<IDapperSiteMapRepository, DapperSiteMapRepository>();
builder.Services.AddTransient<IDapperSlideshowRepository, DapperSlideshowRepository>();
builder.Services.AddTransient<IDapperEmailEnquiryRepository, DapperEmailEnquiryRepository>();
builder.Services.AddTransient<IDapperFAQRepository, DapperFAQRepository>();
builder.Services.AddTransient<IDapperReviewRepository, DapperReviewRepository>();
builder.Services.AddTransient<IDapperStaffRepository, DapperStaffRepository>();

builder.Services.AddTransient<ICalendarRepositoryEF, CalendarRepositoryEF>();
builder.Services.AddTransient<IPropertyRepositoryEF, PropertyRepositoryEF>();
builder.Services.AddTransient<IPageRepositoryEF, PageRepositoryEF>();


// Infrastructure -- Utilities
//builder.Services.AddTransient<IEmailService, EmailService>();
//builder.Services.AddTransient<IFileUploader, FileUploader>();

//builder.Services.AddTransient<IEmailTemplateBuilder, EmailTemplateBuilder>();
//builder.Services.AddTransient<IViewRenderService, ViewRenderService>();
builder.Services.AddTransient<IEmailEnquiryCommand, EmailEnquiryCommand>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Add our new services
builder.Services.AddTransient<LymmHolidayLets.Api.Services.IEmailEnquiryService, LymmHolidayLets.Api.Services.EmailEnquiryService>();
builder.Services.AddTransient<LymmHolidayLets.Api.Services.IRecaptchaValidationService, LymmHolidayLets.Api.Services.RecaptchaValidationService>();
builder.Services.AddTransient<IEmailTemplateBuilder, EmailTemplateBuilder>();
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("SmtpConfig"));

builder.Services.AddTransient<LymmHolidayLets.Domain.Interface.ILogger, NLogger>();


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

var app = builder.Build();

// Configure the HTTP request pipeline.
// http://localhost:5026/scalar/v1
// http://localhost:5026/openapi/v1.json
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("NextJsApp");

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();
