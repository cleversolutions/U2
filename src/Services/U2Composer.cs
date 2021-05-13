using Umbraco.Core.Composing;
using U2.Services;
using Umbraco.Core;

namespace U2.Services
{
    public class U2ServiceComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // component for startup
            composition.Register<U2Service>();
        }
    }
}