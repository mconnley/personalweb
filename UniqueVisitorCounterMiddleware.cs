using Microsoft.AspNetCore.Http.Extensions; 
using personalweb.DataAccess;

namespace personalweb {
    public class UniqueVisitorCounterMiddleware{
        private readonly RequestDelegate _requestDelegate;
        private readonly IConfiguration _configuration;

        public UniqueVisitorCounterMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
        {
            _requestDelegate = requestDelegate;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IDataAccessProvider dataAccessProvider){
            if (!context.Request.GetDisplayUrl().Contains("healthz"))
            {
                var visId = context.Request.Cookies["VisitorId"];
                if (visId == null)
                {
                    context.Response.Cookies.Append("VisitorId", Guid.NewGuid().ToString(), new CookieOptions()
                    {
                        Path = "/"
                    });
                    IncrementCount(dataAccessProvider);
                }
            }
            await _requestDelegate(context);
        }

        private void IncrementCount(IDataAccessProvider dataAccessProvider){
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
    }

    public static class RequestVisitorCountMiddleware{
        public static IApplicationBuilder UseVisitorCount(
            this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<UniqueVisitorCounterMiddleware>();
            }
    }
}