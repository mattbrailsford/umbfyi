namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoMediumRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public UmbracoMediumRssExtractor()
            : base("https://medium.com/feed/tag/umbraco", new[] { "community", "blog" })
        { }
    }
}
