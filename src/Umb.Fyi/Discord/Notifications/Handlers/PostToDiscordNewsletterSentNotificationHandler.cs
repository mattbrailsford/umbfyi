using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umb.Fyi.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Extensions;

namespace Umb.Fyi.Mastodon.Notifications.Handlers
{
    public class PostToDiscordNewsletterSentNotificationHandler : INotificationAsyncHandler<NewsletterSentNotification>
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _config;
        private readonly ILogger<PostToDiscordNewsletterSentNotificationHandler> _logger;

        public PostToDiscordNewsletterSentNotificationHandler(DiscordSocketClient discordSocketClient,
            IConfiguration config,
            ILogger<PostToDiscordNewsletterSentNotificationHandler> logger)
        {
            _discordSocketClient = discordSocketClient;
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(NewsletterSentNotification notification, CancellationToken cancellationToken)
        {
            if (_discordSocketClient.ConnectionState != global::Discord.ConnectionState.Connected)
            {
                _logger.LogDebug("Can't post newsletter to discord as the discord client is not connected");
            }

            try
            {
                var guild = _discordSocketClient.GetGuild(_config.GetValue<ulong>("UmbFyi:Discord:NewsGuildId"));
                var channel = guild.GetTextChannel(_config.GetValue<ulong>("UmbFyi:Discord:NewsChannelId"));

                await channel.SendMessageAsync($@"📰 UMB.FYI {notification.NewsletterNode.Name}: {notification.NewsletterNode.Value<string>("preheader")} {notification.NewsletterNode.Url(mode: Umbraco.Cms.Core.Models.PublishedContent.UrlMode.Absolute)} #umbraco #umbfyi");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't post newsletter to discord");
            }
        }
    }
}
