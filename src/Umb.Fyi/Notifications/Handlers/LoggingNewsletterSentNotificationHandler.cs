using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;

namespace Umb.Fyi.Notifications.Handlers
{
    public class LoggingNewsletterSentNotificationHandler : INotificationAsyncHandler<NewsletterSentNotification>
    {
        public readonly ILogger<LoggingNewsletterSentNotificationHandler> _logger;

        public LoggingNewsletterSentNotificationHandler(ILogger<LoggingNewsletterSentNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(NewsletterSentNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Newsletter Sent: {notification.NewsletterNode.Name} - {notification.NewsletterUrl}");

            return Task.CompletedTask;
        }
    }
}
