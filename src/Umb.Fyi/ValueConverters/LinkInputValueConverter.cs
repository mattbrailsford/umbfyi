using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Templates;

namespace Umb.Fyi.ValueConverters
{
    public class LinkInputValueConverter : TextStringValueConverter
    {
        public LinkInputValueConverter(HtmlLocalLinkParser linkParser, HtmlUrlParser urlParser) 
            : base(linkParser, urlParser)
        { }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias == "linkinput";
    }
}
