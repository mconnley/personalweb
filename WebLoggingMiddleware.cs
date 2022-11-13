using O11yLib;

namespace personalweb {
    public class WebLoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly MyLogger _logger;
        public WebLoggingMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
            _logger = new MyLogger();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.Request.Path == "/")
                {
                    _logger.Info("Got hit",
                        new {
                            HTTPMethod = context.Request?.Method,
                            Path = context.Request?.Path.Value,
                            ResponseCode = context.Response?.StatusCode,
                            IPAddr = context.Connection?.RemoteIpAddress.ToString(),
                            Agent = context.Request?.Headers["User-Agent"].ToString()
                        }
                    );
                }
            }
            catch (System.Exception ex)
            {
                throw;
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
}