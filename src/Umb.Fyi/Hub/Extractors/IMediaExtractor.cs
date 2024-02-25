using Hangfire.Server;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public interface IMediaExtractor
    {
        Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default);
    }
}
