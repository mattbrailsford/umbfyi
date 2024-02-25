using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umb.Fyi.Hub.Migrations
{
    internal class AddMessageColumnToMediaTipTableMigration : MigrationBase
    {
        public AddMessageColumnToMediaTipTableMigration(IMigrationContext context) 
            : base(context)
        { }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", nameof(AddMessageColumnToMediaTipTableMigration));

            var tableName = "fyiMediaTip";
            var columnName = "message";

            if (!ColumnExists(tableName, columnName))
            {
                var nvarcharMaxType = SqlSyntax.GetSpecialDbType(SpecialDbType.NVARCHARMAX);

                Alter.Table(tableName)
                    .AddColumn(columnName).AsCustom(nvarcharMaxType).Nullable()
                    .Do();
            }
        }
    }
}
