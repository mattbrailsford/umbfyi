//using Hangfire.Server;
//using Ical.Net;
//using System.Text.RegularExpressions;
//using Umb.Fyi.Hub.Models;

//namespace Umb.Fyi.Hub.Extractors.Implement
//{
//    public class UmbracoFestivalsICalExtractor : MediaExtractorBase
//    {
//        public DateTime MinPubDate => DateTime.UtcNow.AddDays(-1);

//        private const string IcalUrl = "https://calendar.google.com/calendar/ical/c_57e6d0e5323922d0949fe02afbeca5c2356709489bd2df7ac44a59d3e524d1ac%40group.calendar.google.com/public/basic.ics";
//        private Regex UrlPattern = new Regex("((http|ftp|https):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?)");

//        public UmbracoFestivalsICalExtractor()
//            : base(new[] { "event", "festival" })
//        { }

//        public override async Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
//        {
//            var raw = await FetchAsync(IcalUrl, cancellationToken);
//            var calendar = Calendar.Load(raw);

//            return calendar?.Events.Select(x =>
//            {
//                var urlMatches = UrlPattern.Match(x.Description);
//                if (urlMatches.Success && x.Start.Date.ToUniversalTime() > MinPubDate)
//                {
//                    var url = urlMatches.Groups[1].Value;
//                    var isHq = x.Summary.Contains("codegarden", StringComparison.OrdinalIgnoreCase);
//                    var description = $"{x.Location} - {x.Start.Date.ToUniversalTime().ToString("f")}";

//                    if (x.End != null)
//                    {
//                        description += $" - {x.End.Date.ToUniversalTime().ToString("f")}";
//                    }

//                    return new MediaItem
//                    {
//                        Link = url,
//                        Title = x.Summary,
//                        Description = description,
//                        Date = x.Start.Date.ToUniversalTime(),
//                        Tags = new[] { isHq ? "hq" : "community" }.Concat(Tags).ToArray()
//                    };
//                }
//                else
//                {
//                    return null;
//                }
//            }).Where(x => x != null);
//        }
//    }
//}
