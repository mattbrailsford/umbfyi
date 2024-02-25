using Microsoft.Extensions.Configuration;
using Skybrud.Essentials.Http;
using Umb.Fyi.Mailcoach;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umb.Fyi.Notifications.Handlers
{
    public class NewsletterPublishedNotificationHandler : INotificationAsyncHandler<ContentPublishedNotification>
    {
        private readonly IUmbracoContextFactory _ctxFactory;
        private readonly MailcoachClient _mailcoachClient; 
        protected readonly IConfiguration _config;

        public NewsletterPublishedNotificationHandler(IUmbracoContextFactory ctxFactory,
            MailcoachClient mailcoachClient,
            IConfiguration config)
        {
            _ctxFactory = ctxFactory;
            _mailcoachClient = mailcoachClient;
            _config = config;
        }

        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken)
        {
            using (var ctx = _ctxFactory.EnsureUmbracoContext())
            {
                foreach (var node in notification.PublishedEntities.Where(x => x.ContentType.Alias.InvariantEquals("newsletter")))
                {
                    // Make sure we have a published date and it's set in the future
                    var publishedNode = ctx.UmbracoContext.Content.GetById(node.Id);
                    if (publishedNode.HasValue("publishDate"))
                    {
                        var publishedDate = publishedNode.Value<DateTime>("publishDate");
                        if (publishedDate > DateTime.Now)
                        {
                            var campaign = await GetMailcoachCampaignFromNodeAsync(publishedNode);
                            if (campaign != null) // Update
                            {
                                // Can't update if the campaign is already sent
                                if (campaign.Status != "sent")
                                {
                                    var markup = await GetNewsletterMarkupAsync(publishedNode);

                                    var result = await _mailcoachClient.UpdateCampaignAsync(campaign.Uuid, new MailcoachCampaignRequest
                                    {
                                        EmailListUuid = _config.GetValue<string>("UmbFyi:Mailcoach:ListUuid"),
                                        Name = $"UMB.FYI {publishedNode.Name} [{publishedNode.Key}]",
                                        Subject = $"UMB.FYI {publishedNode.Name}",
                                        Html = markup,
                                        ScheduleAt = publishedDate,
                                        UtmTags = false
                                    });

                                    if (!result.Message.IsNullOrWhiteSpace())
                                    {
                                        notification.Messages.Add(new EventMessage("Mailcoach", result.Message, EventMessageType.Error));
                                    }
                                }
                            }
                            else // Create
                            {
                                var markup = await GetNewsletterMarkupAsync(publishedNode);

                                var result = await _mailcoachClient.CreateCampaignAsync(new MailcoachCampaignRequest
                                {
                                    EmailListUuid = _config.GetValue<string>("UmbFyi:Mailcoach:ListUuid"),
                                    Name = $"UMB.FYI {publishedNode.Name} [{publishedNode.Key}]",
                                    Subject = $"UMB.FYI {publishedNode.Name}",
                                    Html = markup,
                                    ScheduleAt = publishedDate,
                                    UtmTags = false
                                });

                                if (!result.Message.IsNullOrWhiteSpace())
                                {
                                    notification.Messages.Add(new EventMessage("Mailcoach", result.Message, EventMessageType.Error));
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task<MailcoachCampaign> GetMailcoachCampaignFromNodeAsync(IPublishedContent content)
        {
            var resp = await _mailcoachClient.GetCampaignsAsync($"[{content.Key}]");
            if (resp.Data != null && resp.Data.Count > 0)
            {
                return resp.Data.First();
            }

            resp = await _mailcoachClient.GetCampaignsAsync($"{content.Name}");
            if (resp.Data != null && resp.Data.Count > 0)
            {
                return resp.Data.First();
            }

            return null;
        }

        private async Task<string> GetNewsletterMarkupAsync(IPublishedContent content)
        {
            var resp = await HttpRequest.Get(content.Url(mode: UrlMode.Absolute) + "?nl=true")
                .GetResponseAsync().ConfigureAwait(false);

            return resp.Body;
        }
    }
}
