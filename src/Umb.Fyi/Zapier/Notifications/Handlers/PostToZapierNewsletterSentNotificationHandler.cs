using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skybrud.Essentials.Http;
using System.Net;
using Umb.Fyi.Models;
using Umb.Fyi.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Extensions;

namespace Umb.Fyi.Zapier.Notifications.Handlers
{
    public class PostToZapierNewsletterSentNotificationHandler : INotificationAsyncHandler<NewsletterSentNotification>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PostToZapierNewsletterSentNotificationHandler> _logger;

        public PostToZapierNewsletterSentNotificationHandler(IConfiguration config,
            ILogger<PostToZapierNewsletterSentNotificationHandler> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(NewsletterSentNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var newsletter = notification.NewsletterNode as Newsletter;
                if (newsletter != null)
                {
                    var body = JsonConvert.SerializeObject(new
                    {
                        key = newsletter.Key.ToString(),
                        name = newsletter.Name,
                        description = newsletter.Preheader,
                        url = newsletter.Url(mode: Umbraco.Cms.Core.Models.PublishedContent.UrlMode.Absolute)
                    });

                    var resp = await HttpRequest.Post(_config.GetValue<string>("UmbFyi:Zapier:WebhookUrl"))
                        .SetAcceptHeader("application/json")
                        .SetContentType("application/json")
                        .SetBody(body)
                        .GetResponseAsync();

                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError($"Error posting to Zapier webhook: [{resp.StatusCode}] {resp.StatusDescription}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error posting to Zapier webhook");
            }
        }
    }
}
