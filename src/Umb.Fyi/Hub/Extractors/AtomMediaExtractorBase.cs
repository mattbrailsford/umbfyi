using Hangfire.Server;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public abstract class AtomMediaExtractorBase : MediaExtractorBase
    {
        protected AtomMediaExtractorBase(string[] categories) 
            : base(categories)
        { }

        public abstract Task<string> GetFeedUrlAsync(CancellationToken cancellationToken = default);

        public async override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
