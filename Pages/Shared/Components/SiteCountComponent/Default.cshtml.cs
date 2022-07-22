using Microsoft.AspNetCore.Mvc;
using personalweb.Models;
using personalweb.DataAccess;
using Microsoft.Extensions.Logging;

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

        public IViewComponentResult Invoke(object parameter)
        {
            var visitorText = "";
            try
            {
                var model = new SiteCount();
                var SiteKey = _configuration["siteKey"];
                var current = _dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                if (current is null)
                {
                    var s = new SiteCount();
                    s.SiteKey = SiteKey;
                    s.Hits = 0;
                    _dataAccessProvider.AddSiteCountRecord(s);
                    current = _dataAccessProvider.GetSiteCountSingleRecord(SiteKey);
                }
                model.SiteKey = SiteKey;
                model.Hits = current.Hits;
                model.id = current.id;
                visitorText = "You are visitor #" + model.Hits;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Caught error in viewcount db operation.");
                
            }
            return View(visitorText);
        }

        

}