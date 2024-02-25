using Hangfire;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umb.Fyi.Hub.Extractors;
using Umb.Fyi.Hub.Extractors.Implement;
using Umb.Fyi.Hub.Jobs.Implement;

namespace Umb.Fyi.Hub.Composition
{
    [ComposeAfter(typeof(Cultiv.Hangfire.HangfireComposer))]
    public class HangfireComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.WithCollectionBuilder<MediaExtractorCollectionBuilder>()
                .Add(() => builder.TypeLoader.GetTypes<IMediaExtractor>());

            // Blogs
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoBlogRssExtractor>>("UmbracoBlogRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoCommunityBlogsRssExtractor>>("UmbracoCommunityBlogsRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<SkriftBlogScraperExtractor>>("SkriftBlogScraperExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");

            // Blogging platform searches
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoDevToRssExtractor>>("UmbracoDevToRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoHashnodeRssExtractor>>("UmbracoHashnodeRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoMediumRssExtractor>>("UmbracoMediumRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());

            // YouTube
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoYoutubeExtractor>>("UmbracoYoutubeExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoCommunityYoutubeExtractor>>("UmbracoCommunityYoutubeExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());

            // Podcasts
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<CandidContribRssExtractor>>("CandidContribRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<PackageManifestRssExtractor>>("PackageManifestRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<DXTalkRssExtractor>>("DXTalkRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");

            // Events
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoTrainingApiExtractor>>("UmbracoTrainingApiExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracalendarRssExtractor>>("UmbracalendarRssExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");

            // Misc
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoMarketplaceApiExtractor>>("UmbracoMarketplaceApiExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Hourly());
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoRfcsScraperExtractor>>("UmbracoRfcsScraperExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");
            RecurringJob.AddOrUpdate<QueuedMediaExtractorJob<UmbracoAnnouncementsScraperExtractor>>("UmbracoAnnouncementsScraperExtractor",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");

            // Core
            RecurringJob.AddOrUpdate<QueuedMediaProcessorJob>("MediaProcessor",
                x => x.ExecuteAsync(CancellationToken.None),
                Cron.Minutely());
            RecurringJob.AddOrUpdate<MediaCleanupJob>("MediaCleanupJob",
                x => x.ExecuteAsync(CancellationToken.None),
                () => "0 */6 * * *");
        }
    }
}
