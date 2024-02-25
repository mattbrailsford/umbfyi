//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json.Linq;
//using Skybrud.Social.Mastodon;
//using Skybrud.Social.Mastodon.Models;
//using Skybrud.Social.Mastodon.Models.Accounts;
//using Skybrud.Social.Mastodon.Models.Statuses;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Runtime.Serialization;

//namespace Umb.Fyi.Mastodon
//{
//    internal static class MastodonHttpClientExtensions
//    {
//        public static IObservable<IStreamEntity> GetObservable(this MastodonHttpClient client, string url)
//        {
//            return Observable.Create<IStreamEntity>(async (observer, ct) =>
//            {
//                var eventSubject = new Subject<StreamEvent>();
//                var entityBodySubject = new Subject<string>();

//                eventSubject.Zip(entityBodySubject, (e, b) => ConvertStreamEntity(e, b))
//                            .Subscribe(x => observer.OnNext(x));

//                try
//                {
//                    var c2 = new HttpClient(new AuthHttpClientHandler(client.AccessToken))
//                    {
//                        BaseAddress = new Uri($"https://{client.Domain}"),
//                        Timeout = Timeout.InfiniteTimeSpan
//                    };

//                    using (var stream = await c2.GetStreamAsync(url))
//                    using (var sr = new StreamReader(stream))
//                    {
//                        while (!sr.EndOfStream && !ct.IsCancellationRequested)
//                        {
//                            var s = await sr.ReadLineAsync();

//                            if (string.IsNullOrEmpty(s) || s.StartsWith(":"))
//                            {
//                                continue;
//                            }

//                            if (s.StartsWith("event: "))
//                            {
//                                eventSubject.OnNext(StreamEventExtentions.FromString(s.Substring("event: ".Length)));
//                            }
//                            else if (s.StartsWith("data: "))
//                            {
//                                entityBodySubject.OnNext(s.Substring("data: ".Length));
//                            }
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    observer.OnError(ex);
//                    eventSubject.OnError(ex);
//                    entityBodySubject.OnError(ex);
//                    return;
//                }
//                if (!ct.IsCancellationRequested)
//                {
//                    observer.OnCompleted();
//                    eventSubject.OnCompleted();
//                    entityBodySubject.OnCompleted();
//                }
//            });
//        }

//        private static IStreamEntity ConvertStreamEntity(StreamEvent ev, string entityBody)
//        {
//            switch (ev)
//            {
//                //case StreamEvent.Update:
//                //    return JsonSerializer.Deserialize<Status>(entityBody);
//                case StreamEvent.Notification:
//                    return JsonConvert.DeserializeObject<Notification>(entityBody);
//                //case StreamEvent.Delete:
//                //    return new DeletedStream
//                //    {
//                //        StatusId = int.Parse(entityBody)
//                //    };
//                default:
//                    throw new Exception();
//            }
//        }
//    }

//    public interface IStreamEntity
//    { }

//    internal enum StreamEvent
//    {
//        Update,
//        Notification,
//        Delete
//    }

//    //public class BaseMastodonEntity : IBaseMastodonEntity
//    //{
//    //    [JsonIgnore]
//    //    public string RawJson { get; set; }
//    //}

//    //public interface IBaseMastodonEntity
//    //{
//    //    string RawJson { get; set; }
//    //}

//    public class Notification : MastodonObject, IStreamEntity
//    {
//        protected Notification(JObject json) 
//            : base(json)
//        { }

//        [JsonProperty("id")]
//        public int Id { get; set; }

//        [JsonProperty("type")]
//        public NotificationType Type { get; set; }

//        [JsonProperty("created_at")]
//        public DateTime CreatedAt { get; set; }

//        [JsonProperty("account")]
//        public MastodonAccount Account { get; set; }

//        [JsonProperty("status")]
//        public MastodonStatus Status { get; set; }

//    }

//    [JsonConverter(typeof(StringEnumConverter))]
//    public enum NotificationType
//    {
//        [EnumMember(Value = "mention")]
//        Mention,
//        [EnumMember(Value = "reblog")]
//        Reblog,
//        [EnumMember(Value = "favourite")]
//        Favourite,
//        [EnumMember(Value = "follow")]
//        Follow
//    }

//    internal static class StreamEventExtentions
//    {
//        public static StreamEvent FromString(string ev)
//        {
//            switch (ev.ToLower().Trim())
//            {
//                //case "update":
//                //    return StreamEvent.Update;
//                case "notification":
//                    return StreamEvent.Notification;
//                //case "delete":
//                //    return StreamEvent.Delete;
//                default:
//                    throw new NotSupportedException($"Unknown StreamEvent type: {ev}");
//            }
//        }
//    }

//    internal class AuthHttpClientHandler : DelegatingHandler
//    {
//        private string AccessToken;

//        public AuthHttpClientHandler(string accessToken)
//            : this(new HttpClientHandler(), accessToken)
//        { }

//        public AuthHttpClientHandler(HttpMessageHandler handler, string accessToken)
//            : base(handler)
//        {
//            AccessToken = accessToken;
//        }

//        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            request.Headers.Add("Authorization", $"Bearer {AccessToken}");
//            return base.SendAsync(request, cancellationToken);
//        }
//    }
//}
