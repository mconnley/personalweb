using O11yLib;
namespace personalweb;
public class WebLoggingMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private MyLogger? _logger;
    public WebLoggingMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext context, IRequestIPFinder requestIPFinder, MyLogger logger)
    {
        _logger = logger;
        try
        {
            if (context.Request.Path == "/")
            {
                var ip = requestIPFinder.GetIP(context);
                _logger.Info("Got hit",
                    new {
                        HTTPMethod = context.Request?.Method,
                        Path = context.Request?.Path.Value,
                        ResponseCode = context.Response?.StatusCode,
                        IPAddr = ip,
                        Agent = context.Request?.Headers["User-Agent"].ToString()
                    }
                );
            }
        }
        catch (System.Exception ex)
        {
            _logger.Error("Exception occurred in WebLoggingMiddleware", null, ex);
        }
        await _requestDelegate(context);
    }
}

public static class RequestWebLoggingMiddleware {
    public static IApplicationBuilder UseWebLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<WebLoggingMiddleware>();
    }
}
