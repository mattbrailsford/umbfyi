using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umb.Fyi.Discord.Bot;
using Umb.Fyi.Hub;
using Umb.Fyi.Mailcoach;
using Umb.Fyi.Mastodon.Bot;
using Umb.Fyi.Mastodon.Notifications.Handlers;
using Umb.Fyi.Notifications;
using Umb.Fyi.Notifications.Handlers;
using Umb.Fyi.Twitter.Notifications.Handlers;
using Umb.Fyi.Web;
using Umb.Fyi.Zapier.Notifications.Handlers;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Umb.Fyi
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddUmbFyi(this IUmbracoBuilder builder)
        {
            builder.AddUmbFyiHub();

            builder.Services
                .AddSingleton<MailcoachClient>()
                .AddSingleton<UmbFyiDiscordBotSettings>()
                .AddSingleton<DiscordSocketClient>()
                .AddHostedService<UmbFyiDiscordBotService>()
                .AddHostedService<UmbFyiMastodonBotService>()
                .AddHttpClient();

            builder.UrlProviders().Insert<NewsletterUrlProvider>();
            builder.ContentFinders().Insert<NewsletterContentFinder>();

            builder.AddNotificationAsyncHandler<NewsletterSentNotification, LoggingNewsletterSentNotificationHandler>()
                .AddNotificationAsyncHandler<NewsletterSentNotification, PostToMastodonNewsletterSentNotificationHandler>()
                .AddNotificationAsyncHandler<NewsletterSentNotification, PostToDiscordNewsletterSentNotificationHandler>()
                .AddNotificationAsyncHandler<NewsletterSentNotification, PostToTwitterNewsletterSentNotificationHandler>()
                .AddNotificationAsyncHandler<NewsletterSentNotification, PostToZapierNewsletterSentNotificationHandler>()
                .AddNotificationAsyncHandler<ContentPublishedNotification, NewsletterPublishedNotificationHandler>();

            return builder;
        }
    }
}
