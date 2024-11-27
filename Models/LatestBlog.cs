namespace personalweb.Models
{
    public class LatestBlog
    {
        public int? id { get; set; }
        public DateTime? date { get; set; }
        public string? link { get; set; }
        public Title? title { get; set; }
        public Excerpt? excerpt { get; set; }
    }

    public class Title
    {
        public string? rendered { get; set; }
    }

    public class Excerpt
    {
        public string? rendered { get; set; }
    }
}