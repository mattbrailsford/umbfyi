namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoYoutubeExtractor : MultiYoutubeMediaExtractorBase
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public UmbracoYoutubeExtractor() 
            : base(new[] { "hq", "youtube" })
        { }

        public override Task<string[]> GetChannelIdsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new[]{
                "UCcltXlJQ-U553MoOsP9p4wg", // UmbracoHQ
                "UCbGfwSAPflebnadyhEPw-wA"  // UmbracoLearningBase
            });
    }
}
