using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider
{
    public class IdentityProviderRequestedEvent : AuditEvent
    {
        public IdentityProviderDto IdentityProvider { get; set; }

        public IdentityProviderRequestedEvent(IdentityProviderDto identityProvider)
        {
            IdentityProvider = AuditEventDataSanitizer.Sanitize(identityProvider);
        }
    }
}
