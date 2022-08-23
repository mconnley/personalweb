using personalweb.DataAccess;
using Microsoft.EntityFrameworkCore;
using personalweb;
using NLog;
using NLog.Web;
using System.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting application...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorPages();

    builder.Services.AddHealthChecks();

    builder.Configuration.AddJsonFile("secrets/secrets.json", optional: false);

    var connStr = builder.Configuration["SiteCountsConnectionString"];
    builder.Services.AddDbContext<PostgreSqlContext>(options => options.UseNpgsql(connStr));
    builder.Services.AddScoped<IDataAccessProvider, DataAccessProvider>();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddW3CLogging(logging =>
    {
        logging.LoggingFields = W3CLoggingFields.All;
        logging.FileSizeLimit = 5 * 1024 * 1024;
        logging.RetainedFileCountLimit = 2;
        logging.FileName = "personalweb";
        logging.LogDirectory = "/var/log/personalweb";
        logging.FlushInterval = TimeSpan.FromSeconds(2);
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseW3CLogging();
    }

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; style-src-elem 'self' fonts.googleapis.com fonts.gstatic.com; font-src 'self' fonts.googleapis.com fonts.gstatic.com");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=15768000; includeSubDomains; preload");
        await next();
    });

    app.UseHttpsRedirection();

    app.UseVisitorCount();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();

    app.MapHealthChecks("/healthz");
    IHostApplicationLifetime lifetime = app.Lifetime;

    lifetime.ApplicationStopping.Register(() =>
    {
        logger.Info("Waiting to shutdown...");
        Thread.Sleep(5000);
    });

    app.Run();
}
catch (System.Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}