using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Json;
using System.Threading.Tasks;

namespace Umb.Fyi.Mailcoach
{
    public class MailcoachClient
    {
        private string BaseEndpoint = "https://umbfyi.mailcoach.app/api";

        protected readonly IConfiguration _config;

        public MailcoachClient(IConfiguration config)
        {
            _config = config;
        }

        public Task<MailcoachResponse<List<MailcoachCampaign>>> GetCampaignsAsync(string searchTerm)
            => GetAsync<MailcoachResponse<List<MailcoachCampaign>>>($"campaigns", new Dictionary<string, string>
            {
                { "filter[search]", searchTerm }
            });

        public Task<MailcoachResponse<MailcoachCampaign>> GetCampaignAsync(string uuid)
            => GetAsync<MailcoachResponse<MailcoachCampaign>>($"campaigns/{uuid}");

        public Task<MailcoachResponse<MailcoachCampaign>> CreateCampaignAsync(MailcoachCampaignRequest campaign)
            => PostAsync<MailcoachResponse<MailcoachCampaign>>($"campaigns", campaign);

        public Task<MailcoachResponse<MailcoachCampaign>> UpdateCampaignAsync(string uuid, MailcoachCampaignRequest campaign)
            => PutAsync<MailcoachResponse<MailcoachCampaign>>($"campaigns/{uuid}", campaign);

        private async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> qs = null)
        {
            return await HttpRequest.Get($"{BaseEndpoint}/{endpoint}", qs != null ? new HttpQueryString(qs) : null)
                .SetAuthorizationBearer(_config.GetValue<string>("UmbFyi:Mailcoach:ApiKey"))
                .SetAcceptHeader("application/json")
                .GetResponseAsync()
                .AsJson<T>();
        }

        private async Task<T> PostAsync<T>(string endpoint, object payload)
        {
            var body = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });

            var req = HttpRequest.Post($"{BaseEndpoint}/{endpoint}")
                .SetAuthorizationBearer(_config.GetValue<string>("UmbFyi:Mailcoach:ApiKey"))
                .SetContentType("application/json")
                .SetAcceptHeader("application/json")
                .SetBody(body);

            var resp = await req.GetResponseAsync();

            return JsonUtils.ParseJsonToken<T>(resp.Body);
        }

        private async Task<T> PutAsync<T>(string endpoint, object payload)
        {
            var body = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });

            var req = HttpRequest.Put($"{BaseEndpoint}/{endpoint}")
                .SetAuthorizationBearer(_config.GetValue<string>("UmbFyi:Mailcoach:ApiKey"))
                .SetContentType("application/json")
                .SetAcceptHeader("application/json")
                .SetBody(body);

            var resp = await req.GetResponseAsync();

            return JsonUtils.ParseJsonToken<T>(resp.Body);
        }
    }

    public class MailcoachResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class MailcoachCampaign : MailcoachCampaignRequest
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }

    public class MailcoachCampaignRequest
    {
        [JsonProperty("email_list_uuid")]
        public string EmailListUuid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("schedule_at")]
        public DateTime? ScheduleAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("utm_tags")]
        public bool UtmTags { get; set; }
    }
}
