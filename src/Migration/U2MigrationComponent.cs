using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Scoping;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Services;
using Umbraco.Core.Migrations;
using U2.Migration;

namespace U2.Migration
{
    public class U2MigrationComponent : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger _logger;


        public U2MigrationComponent(
            IScopeProvider scopeProvider, 
            IMigrationBuilder migrationBuilder,
            IKeyValueService keyValueService,
            ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }

        public void Initialize()
        {
            // perform any upgrades (as needed)
            var upgrader = new Upgrader(new U2MigrationPlan());
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
}