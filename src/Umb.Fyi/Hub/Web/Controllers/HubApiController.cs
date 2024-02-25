using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Umbraco.Cms.Web.Common.Controllers;
using Umb.Fyi.Hub.Models;
using Umb.Fyi.Hub.Services;

namespace Umb.Fyi.Hub.Web.Controllers
{
    public class HubApiController : UmbracoApiController
    {
        private MediaTipService _tipService;

        public HubApiController(MediaTipService tipService)
        {
            _tipService = tipService;
        }

        [HttpPost]
        public IActionResult Tip(TipRequest tipReq)
        {
            _tipService.SubmitTip(new MediaTip
            {
                Link = tipReq.Link,
                Message = tipReq.Message,
                Source = tipReq.Source
            });

            return Ok();
        }
    }

    public class TipRequest
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
}
