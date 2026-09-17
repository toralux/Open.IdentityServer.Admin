using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.Key
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