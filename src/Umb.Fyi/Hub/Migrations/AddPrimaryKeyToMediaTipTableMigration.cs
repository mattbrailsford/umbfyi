using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umb.Fyi.Hub.Migrations
{
    internal class AddPrimaryKeyToMediaTipTableMigration : MigrationBase
    {
        public AddPrimaryKeyToMediaTipTableMigration(IMigrationContext context) 
            : base(context)
        { }

        protected override void Migrate()
        {
            var isSQLite = DatabaseType == NPoco.DatabaseType.SQLite;
            if (isSQLite) return;

            Logger.LogDebug("Running migration {MigrationStep}", nameof(AddPrimaryKeyToMediaTipTableMigration));

            var tableName = "fyiMediaTip";
            var columnName = "id";
            var pkName = $"Pk_{tableName}";

            if (!PrimaryKeyExists(tableName, pkName))
            {
                Create.PrimaryKey(pkName).OnTable(tableName).Column(columnName).Do();
            }
        }
    }
}
