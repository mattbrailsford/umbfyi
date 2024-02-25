using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Umb.Fyi.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;

namespace Umb.Fyi.Web.Controllers
{
    // TODO: Might actually be better to make some back reference in Umbraco as to
    // the UUID of the campaign and look it up that way?
    // TODO: Need to implement webhook secret validation
    public class MailcoachWebhookApiController : UmbracoApiController
    {
        private readonly IUmbracoContextFactory _ctxFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<MailcoachWebhookApiController> _logger;

        public MailcoachWebhookApiController(IUmbracoContextFactory ctxFactory,
            IEventAggregator eventAggregator,
            ILogger<MailcoachWebhookApiController> logger)
        {
            _ctxFactory = ctxFactory;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CampaignSent(MailcoachCampaignSentEvent payload)
        {
            try
            {
                using (var ctxRef = _ctxFactory.EnsureUmbracoContext())
                {
                    string newsletterName = payload.Name;
                    IPublishedContent newsletterNode = null;

                    // Option 1: Parse the campaign name format '{SUBJECT} [{NODE_KEY}]'
                    var match = Regex.Match(newsletterName, "^(.*)\\s\\[(.*)\\]$", RegexOptions.IgnoreCase);
                    if (match.Success && Guid.TryParse(match.Groups[2].Value, out Guid newsletterId))
                    {
                        newsletterName = match.Groups[1].Value;
                        newsletterNode = ctxRef.UmbracoContext.Content.GetById(newsletterId);
                    }

                    // Option 2: Get all newsletter nodes and look for one with the same name as the campaign
                    if (newsletterNode == null)
                    {
                        var newsletterContentType = ctxRef.UmbracoContext.Content.GetContentType("newsletter");
                        newsletterNode = ctxRef.UmbracoContext.Content.GetByContentType(newsletterContentType)
                            .FirstOrDefault(x => newsletterName.InvariantContains(x.Name));
                    }

                    if (newsletterNode != null)
                    {
                        await _eventAggregator.PublishAsync(new NewsletterSentNotification(newsletterNode,
                            $"https://umbfyi.mailcoach.app/webview/campaign/{payload.Uuid}"));
                    }
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to handle mailcoach webhook notification");
            }

            return Ok();
        }
    }

    public class MailcoachCampaignSentEvent : MailcoachCampaign
    { }

    public class MailcoachCampaign
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
