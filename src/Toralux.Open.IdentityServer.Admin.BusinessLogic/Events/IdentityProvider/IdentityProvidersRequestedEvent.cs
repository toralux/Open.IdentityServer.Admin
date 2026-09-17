using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider
{
    public class IdentityProvidersRequestedEvent : AuditEvent
    {
        public IdentityProvidersDto IdentityProviders { get; set; }

        public IdentityProvidersRequestedEvent(IdentityProvidersDto identityProviders)
        {
            IdentityProviders = AuditEventDataSanitizer.Sanitize(identityProviders);
        }
    }
}
