using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Migrations;
using Umb.Fyi.Hub.Migrations;

namespace Umb.Fyi.Hub.Composition
{
    public class MigrationsComposer : ComponentComposer<MigrationsComponent> { }

    public class MigrationsComponent : IComponent
    {
        private readonly ICoreScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public MigrationsComponent(ICoreScopeProvider scopeProvider, IMigrationPlanExecutor migrationPlanExecutor, IKeyValueService keyValueService, IRuntimeState runtimeState)
        {
            _scopeProvider = scopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            var migrationPlan = new MigrationPlan("Umb.Fyi.Hub");

            migrationPlan.From(string.Empty)
                .To<CreateMediaItemTableMigration>("072f7b82-e592-41fb-ac06-c65ca4ff6a86")
                .To<CreateMediaTipTableMigration>("7699996b-47be-4082-a8bf-26c3c3f0720d")
                .To<AddMessageColumnToMediaTipTableMigration>("82ecda7d-8e9d-4593-9d3d-d0a97d5ac2c8")
                .To<AddPrimaryKeyToMediaItemTableMigration>("d849aa44-0b6b-4ef1-b918-aaa7b505278f")
                .To<AddPrimaryKeyToMediaTipTableMigration>("c6125241-708c-4f19-ad50-d52560954c59");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public void Terminate() { }
    }
}
