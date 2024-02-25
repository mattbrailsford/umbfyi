namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class CandidContribRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public CandidContribRssExtractor() 
            : base("https://www.spreaker.com/show/4200995/episodes/feed", new[] { "community", "podcast" })
        { }
    }
}
