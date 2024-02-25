namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class DXTalkRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public DXTalkRssExtractor() 
            : base("https://feeds.simplecast.com/kzA4I9yY", new[] { "hq", "podcast" })
        { }
    }
}
