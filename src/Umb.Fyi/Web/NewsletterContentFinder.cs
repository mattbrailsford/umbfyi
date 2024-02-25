using System.Text.RegularExpressions;
using Umb.Fyi.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umb.Fyi.Web
{
    public class NewsletterContentFinder : IContentFinder
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public NewsletterContentFinder(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {
            var path = request.Uri.GetAbsolutePathDecoded();
            if (Regex.IsMatch(path, "\\/[0-9]{4}-[0-9]{2}-[0-9]{2}(?:\\/|$)"))
            {
                using (var ctx = _umbracoContextFactory.EnsureUmbracoContext())
                {
                    var node = ctx.UmbracoContext.Content.GetByRoute($"/{Constants.Newsletter.ContainerAlias}{path}");
                    if (node != null)
                    {
                        request.SetPublishedContent(node);
                        return Task.FromResult(true);
                    }
                }
            }
            else if (Regex.IsMatch(path, "\\/latest(?:\\/|$)"))
            {
                using (var ctx = _umbracoContextFactory.EnsureUmbracoContext())
                {
                    var container = ctx.UmbracoContext.Content.GetByRoute($"/{Constants.Newsletter.ContainerAlias}/");
                    if (container != null)
                    {
                        var newsletter = container.Children.OfType<Newsletter>()
                            .Where(x => x.PublishDate < DateTime.UtcNow)
                            .OrderByDescending(x => x.PublishDate)
                            .FirstOrDefault();

                        if (newsletter != null)
                        {
                            request.SetRedirect(newsletter.Url());
                            request.SetNoCacheHeader(true);

                            return Task.FromResult(true);
                        }
                    }
                }

            }

            return Task.FromResult(false);
        }
    }
}
