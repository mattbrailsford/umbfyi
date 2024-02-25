using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.UIBuilder.Configuration.Builders;
using Umbraco.UIBuilder.Events;
using Umbraco.UIBuilder.Extensions;
using Umb.Fyi.Hub.Notifications.Handlers;
using Umb.Fyi.Hub.Mappers;
using Umb.Fyi.Hub.Models;
using Umb.Fyi.Hub.Services;

namespace Umb.Fyi.Hub
{
    public static class UmbracoBuilderExtensions
    {
        public static void ConfigureMediaItemCollection(this TreeCollectionConfigBuilder<MediaItem> colCfg)
        {
            colCfg.SetNameFormat(x => x.Title)
                .AddSearchableProperty(x => x.Title)
                .ShowOnSummaryDashboard()
                .ListView(lvCfg => lvCfg
                    .AddField(x => x.Date, fCfg => fCfg.SetFormat(x => x.ToString("yyyy-MM-dd HH:mm:ss")))
                    .AddField(x => x.Tags, fCfg => fCfg.SetFormat(x => string.Join(", ", x)))
                )
                .Editor(eCfg => eCfg
                    .AddTab("Content", tCfg => tCfg
                        .AddFieldset("Content", fsCfg => fsCfg
                            .AddField(x => x.Link).MakeRequired()
                            .AddField(x => x.Title).MakeRequired()
                            .AddField(x => x.Description).SetDataType("Textarea")
                            .AddField(x => x.Author)
                            .AddField(x => x.Source)
                            .AddField(x => x.Date).SetDataType("Date Picker with time")
                            .AddField(x => x.Tags).SetDataType("Media Hub Tags").SetValueMapper<TagsValueMapper>()
                        )
                    )
                );
        }

        public static IUmbracoBuilder AddUmbFyiHub(this IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<DatabaseCreatedNotification, DatabaseCreatedNotificationHandler>();

            builder.Services.AddSingleton<MediaItemService>();
            builder.Services.AddSingleton<MediaTipService>();

            builder.AddUIBuilder(cfg => cfg
                .AddSectionAfter("media", "Hub", sectionCfg => sectionCfg
                    .Tree(treeCfg => treeCfg
                        .AddCollection<MediaItem>(x => x.Id, "News", "News", "A collection of news items", "icon-newspaper-alt", "icon-newspaper-alt", colCfg => colCfg
                            .SetAlias("news")
                            .SetFilter(x => !x.T.Contains("event"))
                            .SetSortProperty(x => x.Date, Umbraco.UIBuilder.SortDirection.Descending)
                            .ConfigureMediaItemCollection()
                        )
                        .AddCollection<MediaItem>(x => x.Id, "Event", "Events", "A collection of event media items", "icon-calendar", "icon-calendar", colCfg => colCfg
                            .SetAlias("events")
                            .SetFilter(x => x.T.Contains("event"))
                            .SetSortProperty(x => x.Date, Umbraco.UIBuilder.SortDirection.Ascending)
                            .ConfigureMediaItemCollection()
                        )
                        .AddCollection<MediaTip>(x => x.Id, "Tip", "Tips", "A collection of media tips", "icon-pulse", "icon-pulse", colCfg => colCfg
                            .SetNameFormat(x => x.Link)
                            .SetDateCreatedProperty(x => x.Date)
                            .SetSortProperty(x => x.Date, Umbraco.UIBuilder.SortDirection.Descending)
                            .ShowOnSummaryDashboard()
                            .ListView(lvCfg => lvCfg
                                .AddField(x => x.Votes)
                                .AddField(x => x.Date, fCfg => fCfg.SetFormat(x => x.ToString("yyyy-MM-dd HH:mm:ss")))
                            )
                            .Editor(eCfg => eCfg
                                .AddTab("Content", tCfg => tCfg
                                    .AddFieldset("Content", fsCfg => fsCfg
                                        .AddField(x => x.Link).MakeRequired()
                                        .AddField(x => x.Message).SetDataType("Textarea")
                                        .AddField(x => x.Source)
                                    )
                                    .Sidebar(sCfg => sCfg
                                        .AddFieldset("Info", fsCfg => fsCfg
                                            .SetVisibility(x => x.EditorMode == Umbraco.UIBuilder.Configuration.Editors.EditorMode.Edit)
                                            .AddField(x => x.Date).MakeReadOnly(ctx => ctx.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                                            .AddField(x => x.Votes).MakeReadOnly()
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );

            return builder;
        }
    }
}
