namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoCommunityYoutubeExtractor : MultiYoutubeMediaExtractorBase
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);
        public override string[] FilterKeywords => new[] { "umbraco" };

        public UmbracoCommunityYoutubeExtractor() 
            : base(new[] { "community", "youtube" })
        { }

        public override Task<string[]> GetChannelIdsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new[]{
                "UCF_Ene5-58a3Z55aw8O6Djg", // UmbraCoffee
                "UCvWcP8GIYl6l2lJ1Z5-s4ew", // CodesharePaul
                "UCc7FlFtsxY1gLxp1PFf-gqA"  // jondjones
            });
    }
}
