using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umb.Fyi.Web
{
    public class NewsletterUrlProvider : DefaultUrlProvider
    {
        private IUmbracoContextAccessor _umbracoContextAccessor;

        public NewsletterUrlProvider(IOptionsMonitor<RequestHandlerSettings> requestSettings, ILogger<DefaultUrlProvider> logger, ISiteDomainMapper siteDomainMapper, IUmbracoContextAccessor umbracoContextAccessor, UriUtility uriUtility, ILocalizationService localizationService) 
            : base(requestSettings, logger, siteDomainMapper, umbracoContextAccessor, uriUtility, localizationService)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public override UrlInfo GetUrl(IPublishedContent content, UrlMode mode, string culture, Uri current)
        {
            var url = base.GetUrl(content, mode, culture, current);
            var segment = $"/{Constants.Newsletter.ContainerAlias}/";

            if (url.Text.Contains(segment) && !url.Text.EndsWith(segment))
                return UrlInfo.Url(url.Text.Replace(segment, "/"), url.Culture);

            return url;
        }

        public override IEnumerable<UrlInfo> GetOtherUrls(int id, Uri current)
        {
            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
            var node = umbracoContext.Content?.GetById(id);
            if (node != null && node.ContentType.Alias.InvariantEquals("newsletter"))
            {
                var url = GetUrl(node, UrlMode.Relative, null, current).Text;
                url = url + (url.IndexOf("?") >= 0 ? "&" : "?") + "nl=true";
                yield return UrlInfo.Url(url);
            }

            foreach (var otherUrl in base.GetOtherUrls(id, current))
                yield return otherUrl;
        }
    }
}
