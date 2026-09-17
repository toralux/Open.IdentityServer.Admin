using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.Key
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