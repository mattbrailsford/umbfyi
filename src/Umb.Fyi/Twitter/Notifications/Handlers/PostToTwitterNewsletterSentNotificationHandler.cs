using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Http.OAuth;
using Umb.Fyi.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Extensions;

namespace Umb.Fyi.Twitter.Notifications.Handlers
{
    public class PostToTwitterNewsletterSentNotificationHandler : INotificationAsyncHandler<NewsletterSentNotification>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PostToTwitterNewsletterSentNotificationHandler> _logger;

        public PostToTwitterNewsletterSentNotificationHandler(IConfiguration config,
            ILogger<PostToTwitterNewsletterSentNotificationHandler> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(NewsletterSentNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var client = new OAuthClient
                {
                    ConsumerKey = _config.GetValue<string>("UmbFyi:Twitter:ApiKey"),
                    ConsumerSecret = _config.GetValue<string>("UmbFyi:Twitter:ApiKeySecret"),
                    Token = _config.GetValue<string>("UmbFyi:Twitter:AccessToken"),
                    TokenSecret = _config.GetValue<string>("UmbFyi:Twitter:AccessTokenSecret"),
                    RequestTokenUrl = "https://api.twitter.com/oauth/request_token",
                    AuthorizeUrl = "https://api.twitter.com/oauth/authorize",
                    AccessTokenUrl = "https://api.twitter.com/oauth/access_token"
                };

                var data = JObject.FromObject(new
                {
                    text = $@"📰 UMB.FYI {notification.NewsletterNode.Name}: {notification.NewsletterNode.Value<string>("preheader")} {notification.NewsletterNode.Url(mode: Umbraco.Cms.Core.Models.PublishedContent.UrlMode.Absolute)} #umbraco #umbfyi"
                });

                client.Post("https://api.twitter.com/2/tweets", data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't post newsletter to twitter");
            }
        }
    }
}
