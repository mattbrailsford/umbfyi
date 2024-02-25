using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umb.Fyi.Hub.Services;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Skybrud.Essentials.Http;
using Newtonsoft.Json.Linq;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Services;

namespace Umb.Fyi.Web.Controllers
{
    [IsBackOffice]
    [DisableBrowserCache]
    [Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
    public class BackofficeUmbFyiApiController : UmbracoApiController
    {
        private readonly MediaItemService _mediaItemService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BackofficeUmbFyiApiController> _logger;

        public BackofficeUmbFyiApiController(MediaItemService mediaItemService,
            IDataTypeService dataTypeService,
            IConfiguration configuration,
            ILogger<BackofficeUmbFyiApiController> logger)
        {
            _mediaItemService = mediaItemService;
            _dataTypeService = dataTypeService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetMediaItems(string type, DateTime from, DateTime to)
        {
            switch (type)
            {
                case "news":
                    return Ok(_mediaItemService.GetNews(from: from, to: to));
                case "events":
                    return Ok(_mediaItemService.GetEvents(from: from, to: to));
                default:
                    return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeLink(Guid dataTypeKey, string link)
        {
            if (string.IsNullOrWhiteSpace(link) || !link.StartsWith("http"))
                return BadRequest("Invalid link");

            var dataType = _dataTypeService.GetDataType(dataTypeKey);
            if (dataType == null || dataType.EditorAlias != "linkinput")
                return BadRequest("Invalid data type");

            var cfg = dataType.ConfigurationAs<Dictionary<string, object>>();
            if (cfg == null || !cfg.ContainsKey("systemPrompt") || string.IsNullOrWhiteSpace(cfg["systemPrompt"]?.ToString()))
                return BadRequest("Invalid data type config");

            var sr = new SmartReader.Reader(link);
#if DEBUG
            sr.Debug = true;
            sr.LoggerDelegate = (msg) => _logger.LogDebug(msg);
#endif
            var article = await sr.GetArticleAsync();
            if (!article.IsReadable)
                return BadRequest("Unable to extract article body");

            var body = JsonConvert.SerializeObject(new
            {
                model = "gpt-3.5-turbo-1106",
                temperature = 0.3,
                max_tokens = 150,
                messages = new object[] {
                    new {
                        role = "system",
                        content = cfg["systemPrompt"]
                    },
                    new {
                        role = "user",
                        content = article.TextContent
                    }
                }
            });

            var req = HttpRequest.Post("https://api.openai.com/v1/chat/completions")
                .SetAuthorizationBearer(_configuration.GetValue<string>("UmbFyi:OpenAi:SecretKey"))
                .SetContentType("application/json")
                .SetAcceptHeader("application/json")
                .SetBody(body);

            var resp = await req.GetResponseAsync();
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                return BadRequest(resp.StatusDescription);

            var json = JObject.Parse(resp.Body);

            return Ok(new
            {
                link,
                summary = json.SelectToken("choices[0].message.content")?.ToString()
            });
        }
    }
}
