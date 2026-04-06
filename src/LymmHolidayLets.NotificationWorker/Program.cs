using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.NotificationWorker.Consumers;
using LymmHolidayLets.Infrastructure.Emailer;
using MassTransit;
using Serilog;
using SendGrid;
using Twilio;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting LymmHolidayLets.NotificationWorker");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    var config = builder.Configuration;

    // Email infrastructure
    builder.Services.AddSingleton<ISendGridClient>(new SendGridClient(config["SendGrid:ApiKey"]));
    builder.Services.AddTransient<IEmailService, SendGridEmailService>();
    builder.Services.AddTransient<IEmailTemplateBuilder, EmailTemplateBuilder>();
    builder.Services.AddTransient<IEmailGeneratorService, EmailGeneratorService>();

    // SMS infrastructure
    builder.Services.Configure<LymmHolidayLets.NotificationWorker.Services.TwilioOptions>(
        options => config.GetSection("Twilio").Bind(options));
    builder.Services.AddTransient<
        LymmHolidayLets.NotificationWorker.Services.ITextMessageService, 
        LymmHolidayLets.NotificationWorker.Services.TextMessageService>();
    
    // Initialize Twilio client
    var twilioOptions = config.GetSection("Twilio").Get<LymmHolidayLets.NotificationWorker.Services.TwilioOptions>();
    if (twilioOptions != null)
    {
        TwilioClient.Init(twilioOptions.AccountSid, twilioOptions.AuthToken);
    }

    // Configuration bindings
    builder.Services.Configure<SmtpConfig>(options => config.GetSection("SmtpConfig").Bind(options));
    builder.Services.Configure<EmailOptions>(options => config.GetSection("Email").Bind(options));

    // MassTransit with RabbitMQ
    builder.Services.AddMassTransit(x =>
    {
        // Retry with exponential back-off before the message is dead-lettered.
        // Email sending is susceptible to transient failures (SendGrid rate limits,
        // brief network issues) that typically resolve within a few seconds.
        // After 3 retries the message is moved to the *_error queue for inspection.
        x.AddConsumer<EmailEnquiryConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        // Split into two independent consumers so a failure sending one email
        // does not trigger a retry of the other, which would cause duplicate sends.
        // RabbitMQ fans out each BookingConfirmedEvent to both queues independently.
        x.AddConsumer<BookingConfirmedToCompanyConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        x.AddConsumer<BookingConfirmedToCustomerConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        // SMS notifications with same retry pattern as email
        x.AddConsumer<BookingNotificationSmsConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(config["RabbitMQ:Host"] ?? "localhost", "/", h =>
            {
                h.Username(config["RabbitMQ:Username"] ?? "guest");
                h.Password(config["RabbitMQ:Password"] ?? "guest");
            });

            cfg.ConfigureEndpoints(ctx);
        });
    });

    var app = builder.Build();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "LymmHolidayLets.NotificationWorker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
