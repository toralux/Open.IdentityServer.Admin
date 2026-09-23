using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.Key
{
    public class KeysRequestedEvent : AuditEvent
    {
        public KeysDto Keys { get; set; }

        public KeysRequestedEvent(KeysDto keys)
        {
            Keys = keys;
        }
    }
}