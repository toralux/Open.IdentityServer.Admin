using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider
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
