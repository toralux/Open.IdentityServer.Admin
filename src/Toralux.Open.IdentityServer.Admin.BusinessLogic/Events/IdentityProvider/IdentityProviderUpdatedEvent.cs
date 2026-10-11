// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider
{
    public class IdentityProviderUpdatedEvent : AuditEvent
    {
        public IdentityProviderDto OriginalIdentityProvider { get; set; }
        public IdentityProviderDto IdentityProvider { get; set; }

        public IdentityProviderUpdatedEvent(IdentityProviderDto originalIdentityProvider, IdentityProviderDto identityProvider)
        {
            OriginalIdentityProvider = AuditEventDataSanitizer.Sanitize(originalIdentityProvider);
            IdentityProvider = AuditEventDataSanitizer.Sanitize(identityProvider);
        }
    }
}
