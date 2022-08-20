namespace personalweb.Models
{
    public class SiteCount
    {
        public int? id { get; set; }
        public string SiteKey { get; set; } = string.Empty;
        public int Hits { get; set; } = 0;
    }
}