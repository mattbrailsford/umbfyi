namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoDevToRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3); 

        public UmbracoDevToRssExtractor()
            : base("https://dev.to/feed/tag/umbraco", new[] { "community", "blog" })
        { }
    }
}
