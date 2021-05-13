using Umbraco.Core.Migrations;
using U2.Models;

namespace U2.Migration
{
    public class U2Migration : MigrationBase
    {
        public U2Migration(IMigrationContext context) : base(context) { }

        public override void Migrate()
        {
            if (!TableExists("U2UserSettings")) Create.Table<U2UserSettings>().Do();
        }
    }
}