using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extensions;

namespace Umb.Fyi.Hub.Migrations
{
    internal class CreateMediaItemTableMigration : MigrationBase
    {
        public CreateMediaItemTableMigration(IMigrationContext context) 
            : base(context)
        { }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", nameof(CreateMediaItemTableMigration));

            var tableName = "fyiMediaItem";
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToArray();
            if (tables.InvariantContains(tableName)) return;

            var nvarcharMaxType = SqlSyntax.GetSpecialDbType(SpecialDbType.NVARCHARMAX);

            // Create table
            Create.Table(tableName)
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey($"Pk_{tableName}")
                .WithColumn("link").AsString(255).NotNullable()
                .WithColumn("title").AsString(255).NotNullable()
                .WithColumn("description").AsCustom(nvarcharMaxType).Nullable()
                .WithColumn("author").AsString(255).Nullable()
                .WithColumn("source").AsString(255).Nullable()
                .WithColumn("date").AsDateTime().NotNullable()
                .WithColumn("tags").AsString(255).Nullable()
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
