using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider
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
