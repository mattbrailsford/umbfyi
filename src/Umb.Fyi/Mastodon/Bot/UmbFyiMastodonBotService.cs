using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.Mastodon;
using Skybrud.Social.Mastodon.Models;
using Skybrud.Social.Mastodon.Models.Accounts;
using Skybrud.Social.Mastodon.Models.Statuses;
using Skybrud.Social.Mastodon.Options.Statuses;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Umb.Fyi.Common.Bot;
using Umb.Fyi.Hub.Models;
using Umb.Fyi.Hub.Services;
using Websocket.Client;

using JsonUtils = Skybrud.Essentials.Json.Newtonsoft.JsonUtils;

namespace Umb.Fyi.Mastodon.Bot
{
    public class UmbFyiMastodonBotService : BotBase, IHostedService, IDisposable
    {
        private readonly MastodonHttpService _masto;
        private readonly WebsocketClient _websocketClient;
        private readonly ILogger<UmbFyiMastodonBotService> _logger;

        public UmbFyiMastodonBotService(MediaTipService tipService,
            IConfiguration config,
            ILogger<UmbFyiMastodonBotService> logger)
            : base(tipService)
        {
            _logger = logger;

            _masto = MastodonHttpService
                .CreateFromAccessToken(config.GetValue<string>("UmbFyi:Mastodon:Server"),
                config.GetValue<string>("UmbFyi:Mastodon:AccessToken"));

            var wsUrl = new Uri($"wss://{config.GetValue<string>("UmbFyi:Mastodon:Server")}/api/v1/streaming?access_token={config.GetValue<string>("UmbFyi:Mastodon:AccessToken")}&stream=user:notification");

            _websocketClient = new WebsocketClient(wsUrl);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
#if DEBUG
            _websocketClient.DisconnectionHappened.Subscribe(info =>
            {
                if (info.Exception != null)
                {
                    _logger.LogError(info.Exception, $"Mastodon connection lost: " + info.Type.ToString());
                }
                else
                {
                    _logger.LogInformation($"Mastodon connection lost: " + info.Type.ToString());
                }
            });
            _websocketClient.ReconnectionHappened.Subscribe(info => _logger.LogInformation($"Mastodon connection restored: " + info.Type.ToString()));
#endif

            _websocketClient.MessageReceived.Subscribe(msg =>
            {
                var model = JsonUtils.ParseJsonObject(msg.Text, MastodonStreamEvent.Parse);
                if (model.Event == "notification")
                {
                    var notification = JsonUtils.ParseJsonObject(model.Payload, MastodonNotification.Parse);
                    if (notification != null)
                    {
                        HandleNotification(notification);
                    }
                }
            });

            return _websocketClient.Start();
        }

        private void HandleNotification(MastodonNotification notification)
        {
            if (notification.Type == MastodonNotificationType.Mention)
            {
                var status = notification.Status.Content.StripHtml().ToLowerInvariant();
                if (status.IndexOf("@umbfyi tip") >= 0)
                {
                    try
                    {
                        var originalStatus = _masto.Statuses.GetStatus(notification.Status.InReplyToId).Body;

                        var tip = new MediaTip
                        {
                            Link = originalStatus.Url,
                            Message = originalStatus.Content.StripHtml(),
                            Source = originalStatus.Account.Url
                        };

                        PostTip(tip);

                        _masto.Statuses.PostStatus(new MastodonPostStatusOptions
                        {
                            InReplyTo = notification.Status.Id,
                            Visibility = MastodonStatusVisibility.Direct,
                            Status = $@"@{notification.Status.Account.Acct} Thanks for the tip"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error posting Mastodon tip");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_websocketClient.IsStarted)
                _websocketClient.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Hosted Service Stopping");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _websocketClient?.Dispose();
        }
    }

    public class MastodonStreamEvent : JsonObjectBase
    {
        protected MastodonStreamEvent(JObject json) 
            : base(json)
        {
            Stream = json.GetStringArray("stream");
            Event = json.GetString("event");
            Payload = json.GetString("payload");
        }

        public string[] Stream { get; }
        public string Event { get; }
        public string Payload { get; }

        public static MastodonStreamEvent Parse(JObject json)
            => new(json);
    }

    public class MastodonNotification : MastodonObject
    {
        protected MastodonNotification(JObject json)
            : base(json)
        { 
            Id = json.GetInt32("id");
            Type = json.GetEnum<MastodonNotificationType>("type");
            Account = json.GetObject("account", MastodonAccount.Parse);
            Status = json.GetObject("status", MastodonStatus.Parse);
        }

        public int Id { get; }
        public MastodonNotificationType Type { get; }
        public DateTime CreatedAt { get; }
        public MastodonAccount Account { get; }
        public MastodonStatus Status { get; }

        public static MastodonNotification Parse(JObject json)
            => new(json);
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MastodonNotificationType
    {
        [EnumMember(Value = "mention")]
        Mention,

        [EnumMember(Value = "reblog")]
        Reblog,

        [EnumMember(Value = "favourite")]
        Favourite,

        [EnumMember(Value = "follow")]
        Follow
    }
}
