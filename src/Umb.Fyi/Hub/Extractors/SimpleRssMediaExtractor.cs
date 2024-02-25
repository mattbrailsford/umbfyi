namespace Umb.Fyi.Hub.Extractors
{
    public abstract class SimpleRssMediaExtractor : RssMediaExtractorBase
    {
        private string _feedUrl;

        public override Task<string> GetFeedUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_feedUrl);

        public SimpleRssMediaExtractor(string feedUrl, string[] tags)
            : base(tags)
        {
            _feedUrl = feedUrl;
        }
    }
}
