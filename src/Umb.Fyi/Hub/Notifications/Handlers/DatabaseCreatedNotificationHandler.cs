using Umbraco.Cms.Core.Events;
using Umbraco.UIBuilder.Events;
using Umb.Fyi.Hub.Mappers;

namespace Umb.Fyi.Hub.Notifications.Handlers
{
    public class DatabaseCreatedNotificationHandler : INotificationHandler<DatabaseCreatedNotification>
    {
        public void Handle(DatabaseCreatedNotification evt)
        {
            if (evt.Database.DatabaseType.GetProviderName().Contains("SQLite", StringComparison.InvariantCultureIgnoreCase))
                evt.Database.Mappers.Add(new SqliteDataTypeMapper());
        }
    }
}
