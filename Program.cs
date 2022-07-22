using personalweb.DataAccess;
using Microsoft.EntityFrameworkCore;
using personalweb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHealthChecks();

builder.Configuration.AddJsonFile("secrets/secrets.json", optional: false);

var connStr = builder.Configuration.GetConnectionString("SiteCountsContext");
builder.Services.AddDbContext<PostgreSqlContext>(options => options.UseNpgsql(connStr));
builder.Services.AddScoped<IDataAccessProvider, DataAccessProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; style-src-elem 'self' fonts.googleapis.com fonts.gstatic.com; font-src 'self' fonts.googleapis.com fonts.gstatic.com");
    await next();
});

app.UseHsts();

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
    Thread.Sleep(5000);
});


app.Run();
