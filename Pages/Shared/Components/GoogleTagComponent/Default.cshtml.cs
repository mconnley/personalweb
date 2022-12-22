using Microsoft.AspNetCore.Mvc;

namespace Components
{
    public class GoogleTagComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleTagComponent> _logger;
        public GoogleTagComponent(IConfiguration configuration, ILogger<GoogleTagComponent> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IViewComponentResult Invoke()
        {
            string? id = "";
            try
            {
                id = _configuration["googleAnalyticsTagId"];
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to return googleAnalyticsTagId");
            }
            return View(model:id);
        }
    }
}