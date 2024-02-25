using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extensions;

namespace Umb.Fyi.Hub.Migrations
{
    internal class CreateMediaTipTableMigration : MigrationBase
    {
        public CreateMediaTipTableMigration(IMigrationContext context) 
            : base(context)
        { }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", nameof(CreateMediaTipTableMigration));

            var tableName = "fyiMediaTip";
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToArray();
            if (tables.InvariantContains(tableName)) return;

            var nvarcharMaxType = SqlSyntax.GetSpecialDbType(SpecialDbType.NVARCHARMAX);

            // Create table
            Create.Table(tableName)
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey($"Pk_{tableName}")
                .WithColumn("link").AsString(255).NotNullable()
                .WithColumn("message").AsCustom(nvarcharMaxType).Nullable()
                .WithColumn("votes").AsInt32().NotNullable()
                .WithColumn("source").AsString(255).Nullable()
                .WithColumn("date").AsDateTime().NotNullable()
                .Do();


            // Unique link constraint
            Create.Index($"IX_{tableName}_link").OnTable(tableName)
                .OnColumn("link").Ascending()
                .WithOptions().NonClustered()
                .WithOptions().Unique()
                .Do();
        }
    }
}
