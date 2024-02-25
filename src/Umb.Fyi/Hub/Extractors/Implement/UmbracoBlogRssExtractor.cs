namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoBlogRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3); 
        public override string PubDateFormat => "M/d/yyyy h:mm:ss tt";

        public UmbracoBlogRssExtractor()
            : base("https://umbraco.com/rss/blog/", new[] { "hq", "blog" })
        { }
    }
}
