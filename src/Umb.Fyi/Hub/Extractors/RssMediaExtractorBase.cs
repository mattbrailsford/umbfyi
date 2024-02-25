using Hangfire.Console;
using Hangfire.Server;
using Skybrud.Essentials.Time.Rfc2822;
using Skybrud.Essentials.Xml.Extensions;
using System.Globalization;
using System.Xml.Linq;
using Umbraco.Extensions;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors
{
    public abstract class RssMediaExtractorBase : MediaExtractorBase
    {
        public virtual DateTime MinPubDate => DateTime.UtcNow.AddYears(-1);
        public virtual string PubDateFormat => string.Empty;
        public virtual string Source => string.Empty;
        public virtual string Author => string.Empty;
        public virtual string[] FilterKeywords => Array.Empty<string>();

        protected RssMediaExtractorBase(string[] tags) 
            : base(tags)
        { }

        public abstract Task<string> GetFeedUrlAsync(CancellationToken cancellationToken = default);

        public async override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var mediaItems = new List<MediaItem>();

            var feedUrl = await GetFeedUrlAsync(cancellationToken);

            var raw = await FetchAsync(feedUrl, cancellationToken);

            if (string.IsNullOrWhiteSpace(raw))
                return Enumerable.Empty<MediaItem>();

            // Parse the XML into a new instance of XElement
            var feed = XElement.Parse(raw);

            // Make sure it's an RSS feed
            if (feed.Name != "rss")
                return Enumerable.Empty<MediaItem>();

            var channel = feed.Element("channel");
            var channelTitle = channel.Element("title")?.Value;
            var channelLink = channel.Element("link")?.Value ?? feedUrl;

            var items = channel.Elements("item");

            foreach (var item in items)
            {
                var pubDate = GetPubDate(item);
                if (pubDate == default)
                    continue;

                if (pubDate <= MinPubDate)
                    continue;

                var guid = item.Element("guid")?.Value.Trim();
                var link = item.Element("link")?.Value.Trim() ?? guid;

                if (string.IsNullOrWhiteSpace(link))
                    continue;

                var title = item.Element("title")?.Value.Trim() ?? guid;
                var description = item.Element("description")?.Value;
                var author = item.Element("author")?.Value;

                if (FilterKeywords.Length > 0)
                {
                    var includeItem = title.ToLowerInvariant().ContainsAny(FilterKeywords);

                    var categories = item.Elements("category");
                    if (!includeItem && categories.Any())
                    {
                        foreach (var category in categories)
                        {
                            // no need to check more if the item is already approved
                            if (includeItem)
                                continue;

                            foreach (var approvedCategory in FilterKeywords)
                                if (category.Value.ToLowerInvariant().Contains(approvedCategory.ToLowerInvariant()))
                                    includeItem = true;
                        }

                        if (includeItem == false)
                        {
                            if (context != null)
                            {
                                var allCategories = string.Join(",", categories.Select(i => i.Value));
                                context.SetTextColor(ConsoleTextColor.DarkYellow);
                                context.WriteLine($"Not including post titled {title} because it was not in an approved category. The categories it was found in: {allCategories}. [{link}]");
                                context.ResetTextColor();
                            }

                            continue;
                        }
                    }
                }

                mediaItems.Add(new MediaItem
                {
                    Link = link,
                    Title = title,
                    Description = description,
                    Date = pubDate,
                    Author = !string.IsNullOrWhiteSpace(Author) ? Author : author,
                    Source = !string.IsNullOrWhiteSpace(Source) ? Source : channelLink,
                    Tags = Tags //string.Join(",", Categories)
                });
            }

            return mediaItems;
        }

        private DateTime GetPubDate(XElement item)
        {
            var publishDate = item.GetElementValue("pubDate");

            if (!string.IsNullOrWhiteSpace(PubDateFormat))
            {
                if (DateTime.TryParseExact(publishDate, PubDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime pubDate1))
                {
                    return pubDate1.ToUniversalTime();
                }
            }

            if (Rfc2822Utils.TryParse(publishDate, out DateTime pubDate2)) 
            {
                return pubDate2.ToUniversalTime();
            }

            if (DateTime.TryParse(publishDate, out DateTime pubDate3))
            {
                return pubDate3.ToUniversalTime();
            }

            return default;
        }
    }
}
