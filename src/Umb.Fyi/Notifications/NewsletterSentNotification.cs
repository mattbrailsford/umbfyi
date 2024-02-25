using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;

namespace Umb.Fyi.Notifications
{
    public class NewsletterSentNotification : INotification
    {
        public IPublishedContent NewsletterNode { get; }
        public string NewsletterUrl { get; }

        public NewsletterSentNotification(IPublishedContent newsletterNode,
            string newsletterUrl)
        {
            NewsletterNode = newsletterNode;
            NewsletterUrl = newsletterUrl;
        }
    }
}
