namespace personalweb.Models
{
    public class SiteCountView
    {
        public int? id { get; set; }
        public string SiteKey { get; set; } = string.Empty;
        public int Hits { get; set; } = 0;
        public string? VisitorText { get; set; }
    }
}