using Hangfire.Server;
using HtmlAgilityPack;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class SkriftBlogScraperExtractor : MediaExtractorBase
    {
        public virtual DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public SkriftBlogScraperExtractor()
            : base(new[] { "community", "blog", "skrift" })
        { }

        public override Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var mediaItems = new List<MediaItem>();

            var web = new HtmlWeb();
            var htmlDoc = web.Load("https://skrift.io/issues");

            var issues = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'container')]/div[contains(@class, 'issue')]");

            foreach (var issue in issues)
            {
                var timeEl = issue.SelectSingleNode(".//time");
                var articleEls = issue.SelectNodes(".//article");

                var issueNo = issue.GetAttributeValue("id", "");
                var issueDate = DateTime.Parse(timeEl.InnerText.Trim() + " GMT").ToUniversalTime();

                if (issueDate <= MinPubDate)
                    continue;

                foreach (var articleEl in articleEls)
                {
                    var headerEl = articleEl.SelectSingleNode("./h2");
                    var lnkEl = headerEl.SelectSingleNode("./a");

                    var url = "https://skrift.io" + lnkEl.GetAttributeValue("href", "");
                    var title = headerEl.InnerText.Trim();

                    mediaItems.Add(new MediaItem
                    {
                        Link = url,
                        Title = title,
                        Description = "Skrift Issue " + issueNo,
                        Source = "https://skrift.io/",
                        Date = issueDate,
                        Tags = Tags
                    });
                }                
            }

            return Task.FromResult((IEnumerable<MediaItem>)mediaItems);
        }
    }
}
