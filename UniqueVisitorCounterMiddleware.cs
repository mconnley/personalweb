using Microsoft.AspNetCore.Http.Extensions;
using personalweb.DataAccess;
using Components;

namespace personalweb {
    public class UniqueVisitorCounterMiddleware{
        private readonly RequestDelegate _requestDelegate;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UniqueVisitorCounterMiddleware> _logger;

        public UniqueVisitorCounterMiddleware(RequestDelegate requestDelegate, IConfiguration configuration, ILogger<UniqueVisitorCounterMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IDataAccessProvider dataAccessProvider){
            var visitorIdCookieName = _configuration["visitorIdCookieName"];
            if (
                (
                    context.Request.Path == "/" ||
                    context.Request.Path == "/index"
                 ) &&
                !context.Request.Headers.Any(h =>
                    string.Equals(h.Key, _configuration["monitorAddedHeader"], StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                var headers = "";
                foreach (var h in context.Request.Headers)
                {
                    headers += String.Format("--- {0} --- {1} | ", h.Key, h.Value);
                }

                _logger.LogDebug("Headers: {Headers}", headers);
                var visId = context.Request.Cookies[visitorIdCookieName];
                if (visId == null)
                {
                    context.Response.Cookies.Append(visitorIdCookieName, Guid.NewGuid().ToString(), new CookieOptions()
                    {
                        Path = "/"
                    });
                    IncrementCount(dataAccessProvider);
                }
            }
            else {
                _logger.LogDebug("Do not increment hit count");
            }
            await _requestDelegate(context);
        }

        private void IncrementCount(IDataAccessProvider dataAccessProvider){
            try
            {
                var SiteKey = _configuration["siteKey"];
                var current = dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                if (current is null)
                {
                    var s = new Models.SiteCount { SiteKey = SiteKey, Hits = 0};
                    dataAccessProvider.AddSiteCountRecord(s);
                    current = dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                }
                var newhits = current.Hits + 1;
                current.Hits = newhits;
                dataAccessProvider.UpdateSiteCountRecord(current);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Caught exception in Middleware/IncrementCount");
            }
        }
    }

    public static class RequestVisitorCountMiddleware{
        public static IApplicationBuilder UseVisitorCount(
            this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<UniqueVisitorCounterMiddleware>();
            }
    }
}