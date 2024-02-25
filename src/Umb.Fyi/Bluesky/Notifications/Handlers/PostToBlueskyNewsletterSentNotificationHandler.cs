using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umb.Fyi.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Extensions;

namespace Umb.Fyi.Bluesky.Notifications.Handlers
{
    public class PostToBlueskyNewsletterSentNotificationHandler : INotificationAsyncHandler<NewsletterSentNotification>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PostToBlueskyNewsletterSentNotificationHandler> _logger;

        public PostToBlueskyNewsletterSentNotificationHandler(IConfiguration config,
            ILogger<PostToBlueskyNewsletterSentNotificationHandler> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(NewsletterSentNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                //            // Create a session
                //            var sessionBody = JsonConvert.SerializeObject(new
                //            {
                //                identifier = _config.GetValue<string>("UmbFyi:Bluesky:Handle"),
                //                password = _config.GetValue<string>("UmbFyi:Bluesky:AppPassword")
                //            });

                //            var resp = await HttpRequest.Post("https://bsky.social/xrpc/com.atproto.server.createSession")
                //                .SetContentType("application/json")
                //                .SetAcceptHeader("application/json")
                //                .SetBody(sessionBody)
                //                .GetResponseAsync();

                //            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                //                return BadRequest(resp.StatusDescription);

                //            var json = JObject.Parse(resp.Body);
                //            var token = json.SelectToken("accessJwt")?.ToString();

                //            // Upload OG image


                //            // Post a status
                //            var postBody = $@"{{
                //  ""$type"": ""app.bsky.feed.post"",
                //  ""text"": ""📰 UMB.FYI TEST"",
                //  ""createdAt"": ""{DateTime.UtcNow.ToIsoString()}"",
                //  ""embed"": {{
                //    ""$type"": ""app.bsky.embed.external"",
                //    ""external"": {{
                //      ""uri"": ""https://umb.fyi/2023-12-13/"",
                //      ""title"": ""UMB.FYI 2023-12-13"",
                //      ""description"": ""Medium Severity Security Advisory, More 24 Days in Umbraco, CMS trends for 2024"",
                //      ""thumb"": {{
                //        ""$type"": ""blob"",
                //        ""ref"": {{
                //          ""$link"": ""bafkreiash5eihfku2jg4skhyh5kes7j5d5fd6xxloaytdywcvb3r3zrzhu""
                //        }},
                //        ""mimeType"": ""image/png"",
                //        ""size"": 23527
                //      }}
                //    }}
                //  }}
                //}}";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't post newsletter to bluesky");
            }
        }
    }
}
