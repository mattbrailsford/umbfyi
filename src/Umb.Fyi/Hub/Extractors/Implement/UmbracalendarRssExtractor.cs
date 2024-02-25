using Hangfire.Server;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracalendarRssExtractor : SimpleRssMediaExtractor
    {
        public override DateTime MinPubDate => DateTime.UtcNow.AddDays(-1);

        public override string Source => "https://umbracalendar.com";

        public UmbracalendarRssExtractor() 
            : base("https://umbracalendar.com/meetup", new[] { "community", "event" })
        { }

        public override async Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var items = await base.ExtractMediaItemsAsync(context, cancellationToken);

            foreach (var item in items)
            {
                if (item.Link.Contains("meetup.com"))
                {
                    item.Tags = item.Tags.Append("meetup").ToArray();
                }
            }

            return items;
        }
    }
}
