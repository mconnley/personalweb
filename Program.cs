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
    
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(90);
    });

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
        app.UseHsts();
    }

    const string csp = "default-src 'self'; " +
    "connect-src 'self' www.googletagmanager.com https://ampcid.google.com analytics.google.com www.google-analytics.com static.cloudflareinsights.com stats.g.doubleclick.net https://*.clarity.ms; " +
    "font-src 'self' fonts.googleapis.com fonts.gstatic.com; " +
    "frame-src www.googletagmanager.com; " +
    "img-src 'self' https://c.bing.com www.google-analytics.com https://www.google.com/ads/ga-audiences www.googletagmanager.com ssl.gstatic.com www.gstatic.com https://*.clarity.ms stats.g.doubleclick.net/r/; " +
    "style-src 'self' 'unsafe-inline' tagmanager.google.com fonts.googleapis.com; " +
    "style-src-elem 'self' 'unsafe-inline' fonts.googleapis.com fonts.gstatic.com www.googletagmanager.com https://*.clarity.ms; " +
    "script-src 'self' 'unsafe-eval' 'unsafe-inline' ajax.cloudflare.com https://static.cloudflareinsights.com google-analytics.com https://ssl.google-analytics.com www.google-analytics.com tagmanager.google.com googletagmanager.com www.googletagmanager.com https://*.clarity.ms stats.g.doubleclick.net;";

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
    logger.Error("Stopped program because of exception", new object(), ex);
    throw;
}
finally
{
    //NLog.LogManager.Shutdown();
}