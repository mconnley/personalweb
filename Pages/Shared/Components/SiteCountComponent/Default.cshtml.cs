using Microsoft.AspNetCore.Mvc;
using personalweb.Models;
using personalweb.DataAccess;

public class SiteCountComponent : ViewComponent
{
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IConfiguration _configuration;

        public SiteCountComponent(IDataAccessProvider dataAccessProvider, IConfiguration configuration)
        {
            _dataAccessProvider = dataAccessProvider;
            _configuration = configuration;
        }

        public IViewComponentResult Invoke(object parameter)
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
            return View(model);
        }

        

}