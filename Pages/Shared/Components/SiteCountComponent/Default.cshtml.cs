using Microsoft.AspNetCore.Mvc;
using personalweb.Models;
using personalweb.DataAccess;

namespace Components
{
    public class SiteCountComponent : ViewComponent
    {
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SiteCountComponent> _logger;

        public SiteCountComponent(IDataAccessProvider dataAccessProvider, IConfiguration configuration, ILogger<SiteCountComponent> logger)
        {
            _dataAccessProvider = dataAccessProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public IViewComponentResult Invoke()
        {
            var model = new SiteCountView();
            try
            {
                var SiteKey = _configuration["siteKey"];
                if (string.IsNullOrEmpty(SiteKey))
                {
                    throw new Exception("Config value siteKey is null or empty");
                }
                var current = _dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                if (current is null)
                {
                    var s = new SiteCount() { SiteKey = SiteKey, Hits = 0 };
                    _dataAccessProvider.AddSiteCountRecord(s);
                    current = _dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                }
                model.SiteKey = SiteKey;
                model.Hits = current.Hits;
                model.id = current.id;
                model.VisitorText = "You are visitor #" + current.Hits;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Caught error in viewcount db operation.");
            }
            return View(model);
        }
    }
}
