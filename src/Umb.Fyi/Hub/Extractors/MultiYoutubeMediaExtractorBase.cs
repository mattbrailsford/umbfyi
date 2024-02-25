using Hangfire.Server;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public abstract class MultiYoutubeMediaExtractorBase : YoutubeMediaExtractorBase
    {
        private string[] _channelIds;
        private string _currentChannelId;

        protected MultiYoutubeMediaExtractorBase(string[] tags) 
            : base(tags)
        { }

        public abstract Task<string[]> GetChannelIdsAsync(CancellationToken cancellationToken = default);

        public override Task<string> GetChannelIdAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_currentChannelId);

        public async override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            _channelIds = await GetChannelIdsAsync(cancellationToken);

            var allFeedItems = new List<MediaItem>();

            // TODO: Make this a parallel task? but we
            // need to see what happens when setting _currentFeedUrl
            // as we need each execution to know it's current feed url
            foreach (var channelId in _channelIds)
            {
                try
                {
                    _currentChannelId = channelId;

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
