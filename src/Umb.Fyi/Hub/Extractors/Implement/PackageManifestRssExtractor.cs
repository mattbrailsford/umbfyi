namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class PackageManifestRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public PackageManifestRssExtractor() 
            : base("https://feeds.simplecast.com/R6ftAH_u", new[] { "community", "podcast" })
        { }
    }
}
