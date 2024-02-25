using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Options.Statuses;
using Umb.Fyi.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Extensions;

namespace Umb.Fyi.Mastodon.Notifications.Handlers
{
    public class PostToMastodonNewsletterSentNotificationHandler : INotificationAsyncHandler<NewsletterSentNotification>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PostToMastodonNewsletterSentNotificationHandler> _logger;

        public PostToMastodonNewsletterSentNotificationHandler(IConfiguration config,
            ILogger<PostToMastodonNewsletterSentNotificationHandler> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(NewsletterSentNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var mastodon = MastodonHttpService
                .CreateFromAccessToken(_config.GetValue<string>("UmbFyi:Mastodon:Server"),
                _config.GetValue<string>("UmbFyi:Mastodon:AccessToken"));

                await mastodon.Statuses.PostStatusAsync(new MastodonPostStatusOptions
                {
                    Status = $@"📰 UMB.FYI {notification.NewsletterNode.Name}: {notification.NewsletterNode.Value<string>("preheader")} {notification.NewsletterNode.Url(mode:Umbraco.Cms.Core.Models.PublishedContent.UrlMode.Absolute)} #umbraco #umbfyi"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't post newsletter to mastodon");
            }
        }
    }
}
