namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoHashnodeRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3); 

        public UmbracoHashnodeRssExtractor()
            : base("https://hashnode.com/n/umbraco/rss", new[] { "community", "blog" })
        { }
    }
}
