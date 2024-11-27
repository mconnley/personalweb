using Microsoft.AspNetCore.Mvc;
using personalweb.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Components
{
    public class LatestBlogComponent: ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LatestBlogComponent> _logger;
         private readonly IDatabase _redis;
        public LatestBlogComponent(IConfiguration configuration, ILogger<LatestBlogComponent> logger, IConnectionMultiplexer muxer)
        {
            _configuration = configuration;
            _logger = logger;
            _redis = muxer.GetDatabase();
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
                string? json;
                string keyName = "latest_blog";
                json = await _redis.StringGetAsync(keyName);
                if (string.IsNullOrEmpty(json))
                {
                    using var client = new HttpClient(handler);
                    using HttpResponseMessage? resp = await client.GetAsync(_configuration["latestBlogUrl"]);
                    json = await resp.Content.ReadAsStringAsync();
                    var setTask = _redis.StringSetAsync(keyName, json);
                    var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromHours(12));
                }
                
                if (json is not null)
                {
                    var l = JsonConvert.DeserializeObject<List<LatestBlog>>(json);
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
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Caught error in get latest blog operation.");
            }

            return View(model);
        }
    }
}