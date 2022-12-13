namespace personalweb.Models
{
    public class LatestBlogView
    {
        public int? id { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? TitleRendered { get; set; }
        public string? Link { get; set; }
        public string? ExcerptRendered {get; set; }
    }
}