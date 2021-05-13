using Umbraco.Core;
using Umbraco.Core.Composing;

namespace U2.Migration
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class U2MigrationComposer : ComponentComposer<U2MigrationComponent>
    {
    }
}