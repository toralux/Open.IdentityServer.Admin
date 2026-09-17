using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.Key
{
    public class KeyRequestedEvent : AuditEvent
    {
        public KeyDto Key { get; set; }

        public KeyRequestedEvent(KeyDto key)
        {
            Key = key;
        }
    }
}