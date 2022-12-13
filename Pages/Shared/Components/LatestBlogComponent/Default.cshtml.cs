using Microsoft.AspNetCore.Mvc;
using personalweb.Models;
using Newtonsoft.Json;

namespace Components
{
    public class LatestBlogComponent: ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LatestBlogComponent> _logger;
        public LatestBlogComponent(IConfiguration configuration, ILogger<LatestBlogComponent> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new LatestBlogView();
            try
            {
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
                List<LatestBlog> blogList = new();
                using var client = new HttpClient(handler);
                using HttpResponseMessage? resp = await client.GetAsync(_configuration["latestBlogUrl"]);
                if (resp is not null)
                {
                    string apiResp = await resp.Content.ReadAsStringAsync();
                    if (apiResp is not null)
                    {
                        var l = JsonConvert.DeserializeObject<List<LatestBlog>>(apiResp);
                        if (l?.Count > 0)
                        {
                            LatestBlog? blog = l.FirstOrDefault();
                            if (blog is not null)
                            {
                                model.id = blog.id;
                                model.PublishDate = blog.date;
                                model.Link = blog.link;
                                if (blog.title is not null) model.TitleRendered = blog.title.rendered;
                                if (blog.excerpt?.rendered is not null) {
                                    string? exerptWithLink = blog.excerpt.rendered.Replace("</p>","<a href=" + blog.link + "> ... [ Read More! ]</a></p>" );
                                    model.ExcerptRendered = exerptWithLink;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Caught error in get latest blog operation.");
            }

            return View(model);
        }
    }
}