// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider
{
    public class IdentityProviderDeletedEvent : AuditEvent
    {
        public IdentityProviderDto IdentityProvider { get; set; }

        public IdentityProviderDeletedEvent(IdentityProviderDto identityProvider)
        {
            IdentityProvider = AuditEventDataSanitizer.Sanitize(identityProvider);
        }
    }
}
