using Umbraco.Cms.Core.Composing;

namespace Umb.Fyi.Hub.Extractors
{
    public class MediaExtractorCollectionBuilder : LazyCollectionBuilderBase<MediaExtractorCollectionBuilder, MediaExtractorCollection, IMediaExtractor>
    {
        protected override MediaExtractorCollectionBuilder This => this;
    }
}
