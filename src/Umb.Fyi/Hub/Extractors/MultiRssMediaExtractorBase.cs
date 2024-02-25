using Hangfire.Server;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public abstract class MultiRssMediaExtractorBase : RssMediaExtractorBase
    {
        private string[] _feedUrls;
        private string _currentFeedUrl;

        protected MultiRssMediaExtractorBase(string[] tags) 
            : base(tags)
        { }

        public abstract Task<string[]> GetFeedUrlsAsync(CancellationToken cancellationToken = default);

        public override Task<string> GetFeedUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_currentFeedUrl);

        public async override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            _feedUrls = await GetFeedUrlsAsync(cancellationToken);

            var allFeedItems = new List<MediaItem>();

            // TODO: Make this a parallel task? but we
            // need to see what happens when setting _currentFeedUrl
            // as we need each execution to know it's current feed url
            foreach (var feedUrl in _feedUrls)
            {
                try
                {
                    _currentFeedUrl = feedUrl;

                    var feedItems = await base.ExtractMediaItemsAsync(context, cancellationToken);

                    allFeedItems.AddRange(feedItems);
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
            }

            return allFeedItems;
        }
    }
}
