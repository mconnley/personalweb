using Microsoft.AspNetCore.Mvc;

namespace Components
{
    public class GoogleTagMgrBodyComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleTagMgrBodyComponent> _logger;
        public GoogleTagMgrBodyComponent(IConfiguration configuration, ILogger<GoogleTagMgrBodyComponent> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IViewComponentResult Invoke()
        {
            string? id = "";
            try
            {
                id = _configuration["googleTagsId"];
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to return googleTagsId");
            }
            return View(model:id);
        }
    }
}