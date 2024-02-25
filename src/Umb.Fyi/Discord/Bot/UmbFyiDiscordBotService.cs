using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Umb.Fyi.Common.Bot;
using Umb.Fyi.Hub.Models;
using Umb.Fyi.Hub.Services;
using Umbraco.Extensions;

namespace Umb.Fyi.Discord.Bot
{
    public class UmbFyiDiscordBotService : BotBase, IHostedService, IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly UmbFyiDiscordBotSettings _settings;
        private readonly ILogger<UmbFyiDiscordBotService> _logger;

        public UmbFyiDiscordBotService(DiscordSocketClient client,
            UmbFyiDiscordBotSettings settings,
            MediaTipService tipService,
            ILogger<UmbFyiDiscordBotService> logger)
            : base(tipService)
        {
            _client = client;
            _settings = settings;
            _logger = logger;
        }

        // logging
        private async Task Log(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(arg.Exception, arg.Message);
                    return;
                case LogSeverity.Error:
                    _logger.LogError(arg.Exception, arg.Message);
                    return;
                case LogSeverity.Warning:
                    _logger.LogWarning(arg.Exception, arg.Message);
                    return;
                case LogSeverity.Info:
                    _logger.LogInformation(arg.Exception, arg.Message);
                    return;
                case LogSeverity.Verbose:
                    _logger.LogTrace(arg.Exception, arg.Message);
                    return;
                case LogSeverity.Debug:
                    _logger.LogDebug(arg.Exception, arg.Message);
                    return;
            }
        }

        // bot has connected and it's ready to work
        private async Task Ready()
        {
            // set status to online
            await _client.SetStatusAsync(UserStatus.Online);
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            // Has to mention the bot
            if (!msg.Content.Contains($"<@{_client.CurrentUser.Id}>"))
                return;

            // See if it's a tip
            if (msg.Content.InvariantContains($"<@{_client.CurrentUser.Id}> tip"))
            {
                if (msg.Channel.GetChannelType() == ChannelType.PublicThread)
                {
                    var threadChannel = msg.Channel as SocketThreadChannel;
                    var txtChannel = (threadChannel.ParentChannel as SocketTextChannel) ?? threadChannel;

                    var starterMsg = await txtChannel
                           .GetMessageAsync(threadChannel.Id)
                           .ConfigureAwait(false);

                    var tip = new MediaTip
                    {
                        Link = $"https://discord.com/channels/{txtChannel.Guild.Id}/{starterMsg.Id}",
                        Message = starterMsg.CleanContent
                    };

                    await PostTipAsync(tip);

                    await msg.Channel.SendMessageAsync("Thanks for the tip", messageReference: new MessageReference(msg.Id))
                        .ConfigureAwait(false);
                }
                else if (msg.Type == MessageType.Reply)
                {
                    var originalMsg = await msg.Channel.GetMessageAsync(msg.Reference.MessageId.Value)
                        .ConfigureAwait(false);

                    var tip = new MediaTip
                    {
                        Link = $"https://discord.com/channels/{msg.Reference.GuildId}/{msg.Reference.ChannelId}/{msg.Reference.MessageId}",
                        Message = originalMsg?.CleanContent
                    };

                    await PostTipAsync(tip);

                    await msg.Channel.SendMessageAsync("Thanks for the tip", messageReference: new MessageReference(msg.Id))
                        .ConfigureAwait(false);
                }

            }
            else
            {
                await msg.Channel.SendMessageAsync("Sorry, I couldn't record this tip", messageReference: new MessageReference(msg.Id))
                    .ConfigureAwait(false);
            }
        }  

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Ready += Ready;
            _client.MessageReceived += MessageReceived;
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, _settings.Token);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.SetStatusAsync(UserStatus.Offline);
            await _client.StopAsync();

            _client.Log -= Log;
            _client.Ready -= Ready;
            _client.MessageReceived -= MessageReceived;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
