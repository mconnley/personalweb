using personalweb.DataAccess;
using Microsoft.EntityFrameworkCore;
using personalweb;
using System.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using O11yLib;
using NLog;

var logger = new MyLogger();
//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting application...", new {BlahBlah = "bleeeblah", myArg = new {foo = 1234, bar = "baz", boop = true }});
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
    builder.Services.AddScoped<IRequestIPFinder, RequestIPFinder>();
    builder.Services.AddScoped<MyLogger, MyLogger>();
    builder.Logging.ClearProviders();
    //builder.Host.UseNLog();

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

    const string csp = "default-src 'self'; " +
    "style-src-elem 'self' fonts.googleapis.com fonts.gstatic.com www.googletagmanager.com; " +
    "style-src 'self' www.googletagmanager.com; " +
    "font-src 'self' fonts.googleapis.com fonts.gstatic.com; " +
    "script-src 'self' www.googletagmanager.com www.google-analytics.com static.cloudflareinsights.com 'sha256-Zj6tAEuGHORkcFg//Mecf8qY7fYErbxO1toHp7a1FNg=' 'sha256-cbuyYQIXVmwj6nfGeovJBxf2xIamnPjeYTDjWlPC2fk='; " +
    "connect-src 'self' www.googletagmanager.com analytics.google.com www.google-analytics.com static.cloudflareinsights.com stats.g.doubleclick.net";

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("Content-Security-Policy", csp);
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=15768000; includeSubDomains; preload");
        await next();
    });

    app.UseHttpsRedirection();

    app.UseVisitorCount();

    app.UseWebLoggingMiddleware();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();

    app.MapHealthChecks("/healthz");
    IHostApplicationLifetime lifetime = app.Lifetime;

    lifetime.ApplicationStopping.Register(() =>
    {
        //logger.Info("Waiting to shutdown...");
        Thread.Sleep(5000);
    });

    app.Run();
}
catch (System.Exception ex)
{
    logger.Error("Stopped program because of exception", null, ex);
    throw;
}
finally
{
    //NLog.LogManager.Shutdown();
}