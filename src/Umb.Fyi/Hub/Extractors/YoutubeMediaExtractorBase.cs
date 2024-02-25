using Hangfire.Console;
using Hangfire.Server;
using Skybrud.Essentials.Xml.Extensions;
using System.Xml.Linq;
using Umbraco.Extensions;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public abstract class YoutubeMediaExtractorBase : MediaExtractorBase
    {
        public virtual DateTime MinPubDate => DateTime.UtcNow.AddYears(-1);
        public virtual string[] FilterKeywords => Array.Empty<string>();

        protected YoutubeMediaExtractorBase(string[] tags) 
            : base(tags)
        { }

        /// <summary>
        /// Use https://www.streamweasels.com/tools/youtube-channel-id-and-user-id-convertor/ to get channel id
        /// </summary>
        public abstract Task<string> GetChannelIdAsync(CancellationToken cancellationToken = default);

        public async override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var mediaItems = new List<MediaItem>();

            var channelId = await GetChannelIdAsync(cancellationToken);
            var feedUrl = $"https://www.youtube.com/feeds/videos.xml?channel_id={channelId}";

            var raw = await FetchAsync(feedUrl, cancellationToken);

            if (string.IsNullOrWhiteSpace(raw))
                return Enumerable.Empty<MediaItem>();

            // Parse the XML into a new instance of XElement
            var feed = XElement.Parse(raw);

            XNamespace atomNs = "http://www.w3.org/2005/Atom";
            XNamespace mediaNs = "http://search.yahoo.com/mrss/";

            // Make sure it's an ATOM feed
            if (feed.Name != atomNs + "feed")
                return Enumerable.Empty<MediaItem>();

            //var channel = feed.Element("channel");
            //var channelTitle = channel.Element("title")?.Value;
            var feedLink = feed.Elements(atomNs + "link")
                .FirstOrDefault(el => el.Attribute("rel")?.Value == "alternate")
                ?.Attribute("href")
                ?.Value;

            var items = feed.Elements(atomNs + "entry");

            foreach (var item in items)
            {
                var rawPubDate = item.GetElementValue(atomNs + "published");
                if (!DateTime.TryParse(rawPubDate, out DateTime pubDate))
                    continue;

                if (pubDate <= MinPubDate)
                    continue;

                var link = item.Element(atomNs + "link").Attribute("href")?.Value;
                var title = item.Element(atomNs + "title")?.Value;
                var author = item.Element(atomNs + "author")?.Element(atomNs + "name")?.Value;

                var group = item.Element(mediaNs + "group");
                var description = group?.Element(mediaNs + "description")?.Value;

                if (FilterKeywords.Length > 0)
                {
                    var includeItem = title.ToLowerInvariant().ContainsAny(FilterKeywords)
                        || (description + "").ToLowerInvariant().ContainsAny(FilterKeywords);

                    if (includeItem == false)
                    {
                        if (context != null)
                        {
                            context.SetTextColor(ConsoleTextColor.DarkYellow);
                            context.WriteLine($"Not including youtube video titled {title} because it was not in an approved category. [{link}]");
                            context.ResetTextColor();
                        }
                        continue;
                    }
                }

                mediaItems.Add(new MediaItem
                {
                    Link = link,
                    Title = title,
                    Description = description,
                    Date = pubDate.ToUniversalTime(),
                    Author = author,
                    Source = feedLink,
                    Tags = Tags //string.Join(",", Categories)
                });
            }

            return mediaItems;
        }
    }
}
