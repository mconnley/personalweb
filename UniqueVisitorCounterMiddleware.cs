using Microsoft.AspNetCore.Http.Extensions; 
using personalweb.DataAccess;

namespace personalweb {
    public class UniqueVisitorCounterMiddleware{
        private readonly RequestDelegate _requestDelegate;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SiteCountComponent> _logger;

        public UniqueVisitorCounterMiddleware(RequestDelegate requestDelegate, IConfiguration configuration, ILogger<SiteCountComponent> logger)
        {
            _requestDelegate = requestDelegate;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IDataAccessProvider dataAccessProvider){
            var visitorIdCookieName = _configuration["visitorIdCookieName"];
            if (!context.Request.GetDisplayUrl().Contains(_configuration["monitorUrl"]) && !context.Request.Headers.Any(h => h.Key == _configuration["monitorAddedHeader"]))
            {
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
            await _requestDelegate(context);
        }

        private void IncrementCount(IDataAccessProvider dataAccessProvider){
            try
            {
                var SiteKey = _configuration["siteKey"];
                var current = dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                if (current is null)
                {
                    var s = new Models.SiteCount();
                    s.SiteKey = SiteKey;
                    s.Hits = 0;
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