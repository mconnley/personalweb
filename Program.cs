using personalweb.DataAccess;
using Microsoft.EntityFrameworkCore;
using personalweb;
using System.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using O11yLib;
using NLog;
using StackExchange.Redis;

var logger = new MyLogger();

try
{
    if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) && 
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            while (!Debugger.IsAttached) { Thread.Sleep(200); }
        }
    logger.Info("Starting...", new object());
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddRazorPages();

    builder.Services.AddHealthChecks();

    builder.Configuration.AddJsonFile("secrets/secrets.json", optional: false);

    var connStr = builder.Configuration["SiteCountsConnectionString"];
    builder.Services.AddDbContext<PostgreSqlContext>(options => options.UseNpgsql(connStr));
    builder.Services.AddScoped<IDataAccessProvider, DataAccessProvider>();
    builder.Services.AddScoped<IRequestIPFinder, RequestIPFinder>();
    builder.Services.AddScoped<MyLogger, MyLogger>();
    builder.Logging.ClearProviders();

     var redisHostname = builder.Configuration["redisHostname"];
     if (redisHostname == null)
     {
        throw new ArgumentNullException("redisHostname is Null");
     }

     ConfigurationOptions co = new ConfigurationOptions()
     {
        SyncTimeout = 500,
        EndPoints =
        {
            {redisHostname, 6379}
        },
        AbortOnConnectFail = false
     };



    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(co));
 
    
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

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        logger.Info("Running in prod mode...", new object());
        app.UseExceptionHandler("/Error");
        app.UseW3CLogging();
        app.UseForwardedHeaders();
        app.UseHsts();
    }

    const string csp = "default-src 'none'; " +
        "script-src 'self' https://*.cloudflareinsights.com https://*.clarity.ms https://*.google-analytics.com https://*.googletagmanager.com " +
        "'sha256-Zj6tAEuGHORkcFg//Mecf8qY7fYErbxO1toHp7a1FNg=' 'sha256-cbuyYQIXVmwj6nfGeovJBxf2xIamnPjeYTDjWlPC2fk=' 'sha256-+fgVaucvkGqts851ZCDGqdYanjfg3Nk2EiDmGHMUNXE='; " +
        "style-src 'self' https://*.googleapis.com 'sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU='; " +
        "style-src-elem 'self' https://*.googleapis.com 'sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU='; " +
        "object-src 'none';  " +
        "base-uri 'self';  " +
        "connect-src 'self' https://*.google.com https://*.clarity.ms https://*.google-analytics.com https://*.doubleclick.net; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "frame-src 'self'; " +
        "img-src 'self' https://*.bing.com https://*.clarity.ms; " +
        "manifest-src 'self'; " +
        "media-src 'self'; " +
        "worker-src 'none'; " +
        "frame-ancestors 'none'; " +
        "form-action 'none';";

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("Content-Security-Policy", csp);
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=15768000; includeSubDomains; preload");
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
        logger.Info("Waiting to shutdown...", new object());
        Thread.Sleep(5000);
    });
    logger.Info("Config complete.", new object());
    app.Run();
    logger.Info("Started", new object());
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