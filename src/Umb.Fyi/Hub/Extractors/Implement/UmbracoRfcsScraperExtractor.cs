using Hangfire.Server;
using HtmlAgilityPack;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoRfcsScraperExtractor : MediaExtractorBase
    {
        public virtual DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public UmbracoRfcsScraperExtractor() 
            : base(new[] { "hq", "rfc" })
        { }

        public override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var mediaItems = new List<MediaItem>();

            var web = new HtmlWeb();
            var htmlDoc = web.Load("https://github.com/umbraco/rfcs/discussions");

            var items = htmlDoc.DocumentNode.SelectNodes("//ul[@aria-labelledby=\"discussions-list\"]/li");

            foreach (var item in items)
            {
                var lnkEl = item.SelectSingleNode(".//a[@data-hovercard-type=\"discussion\"]");
                var timeEl = item.SelectSingleNode(".//relative-time");

                var url = "https://github.com" + lnkEl.GetAttributeValue("href", "");
                var title = lnkEl.InnerText.Trim();
                var date = DateTime.Parse(timeEl.GetAttributeValue("datetime", "")).ToUniversalTime();

                if (date <= MinPubDate)
                    continue;

                mediaItems.Add(new MediaItem
                {
                    Link = url,
                    Title = title,
                    Source = "https://github.com/umbraco/rfcs/",
                    Date = date,
                    Tags = Tags
                });
            }

            return Task.FromResult((IEnumerable<MediaItem>)mediaItems);
        }
    }
}
