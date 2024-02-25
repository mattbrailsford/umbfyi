using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Services
{
    public class MediaItemService
    {
        private readonly IScopeProvider _scopeProvider;
        public MediaItemService(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public IEnumerable<MediaItem> GetNews(string[] categories = null, DateTime? from = null, DateTime? to = null)
        {
            return GetMediaItems(categories, from, to, sql => sql
                .Where("tags NOT LIKE @0", "%event%")
                .OrderByDescending<MediaItem>(x => x.Date)).ToList();
        }

        public IEnumerable<MediaItem> GetEvents(string[] categories = null, DateTime? from = null, DateTime? to = null)
        {
            return GetMediaItems(categories, from, to, sql => sql
                .Where("tags LIKE @0", "%event%")
                .OrderBy<MediaItem>(x => x.Date)).ToList();
        }

        private IEnumerable<MediaItem> GetMediaItems(string[] categories = null, DateTime? from = null, DateTime? to = null, Func<Sql<ISqlContext>, Sql<ISqlContext>> additionalSql = null)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql()
                    .Select("*")
                    .From<MediaItem>();

                if (categories != null && categories.Length > 0)
                {
                    foreach (var category in categories)
                    {
                        sql = sql.Where("tags LIKE @0", $"%{category}%");
                    }
                }

                if (from != null)
                {
                    sql = sql.Where<MediaItem>(x => x.Date >= from);
                }

                if (to != null)
                {
                    if (to.Value.Hour == 0 && to.Value.Minute == 0 && to.Value.Second == 0)
                        to = to.Value.AddDays(1).AddSeconds(-1);

                    sql = sql.Where<MediaItem>(x => x.Date <= to);
                }

                if (additionalSql != null)
                {
                    sql = additionalSql(sql);
                }

                return scope.Database.Fetch<MediaItem>(sql);
            }
        }
    }
}
