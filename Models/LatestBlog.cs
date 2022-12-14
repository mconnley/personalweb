namespace personalweb.Models
{
    public class LatestBlog
    {
        public int? id { get; set; }
        public DateTime? date { get; set; }
        public string? link { get; set; }
        public title? title { get; set; }
        public excerpt? excerpt { get; set; }
    }

    public class title
    {
        public string? rendered { get; set; }
    }

    public class excerpt
    {
        public string? rendered { get; set; }
    }
}