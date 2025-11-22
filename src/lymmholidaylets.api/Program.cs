using lymmholidaylets.api.GraphQL;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Infrastructure;
using LymmHolidayLets.Infrastructure.DataAdapter;
using LymmHolidayLets.Infrastructure.DependencyInjection;
using LymmHolidayLets.Infrastructure.Repository;
using LymmHolidayLets.Infrastructure.Repository.Dapper;
using LymmHolidayLets.Infrastructure.Repository.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiGraphQL();

builder.Services.AddTransient<ICalendarQuery, CalendarQuery>();

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


// Infrastructure -- Utilities
//builder.Services.AddTransient<IEmailService, EmailService>();
//builder.Services.AddTransient<IFileUploader, FileUploader>();

//builder.Services.AddTransient<IEmailTemplateBuilder, EmailTemplateBuilder>();
//builder.Services.AddTransient<IViewRenderService, ViewRenderService>();

// register EF Core with SQL logging (development)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LymmHolidayLets"))
           // Log SQL commands (filter to database command category to reduce noise)
           .LogTo(Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.DbLoggerCategory.Database.Command.Name }, 
                                  Microsoft.Extensions.Logging.LogLevel.Information)
           // Show parameter values in logs (ONLY for development / debugging)
           .EnableSensitiveDataLogging()
           // Provide more detailed errors for debugging
           .EnableDetailedErrors());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();
