using Umbraco.Cms.Core.Composing;

namespace Umb.Fyi.Hub.Extractors
{
    public class MediaExtractorCollection : BuilderCollectionBase<IMediaExtractor>
    {
        public MediaExtractorCollection(Func<IEnumerable<MediaExtractorBase>> items) 
            : base(items)
        { }
    }
}
