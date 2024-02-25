using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Jobs.Implement
{
    public class MediaCleanupJob : RecuringJobBase
    {
        private IScopeProvider _scopeProvider;

        public MediaCleanupJob(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    // Media
                    var minDate = DateTime.UtcNow.AddMonths(-6);

                    var sql = scope.SqlContext.Sql()
                        .Delete<MediaItem>()
                        .Where<MediaItem>(x => !x.T.Contains("event"))
                        .Where<MediaItem>(x => x.Date < minDate);

                    scope.Database.Execute(sql);

                    // Events
                    minDate = DateTime.UtcNow.AddDays(-1);

                    sql = scope.SqlContext.Sql()
                        .Delete<MediaItem>()
                        .Where<MediaItem>(x => x.T.Contains("event"))
                        .Where<MediaItem>(x => x.Date < minDate);

                    scope.Database.Execute(sql);

                    scope.Complete();
                }
            }
            catch
            {
                // If one file fails, keep trying others
            }

            return Task.CompletedTask;
        }
    }
}
