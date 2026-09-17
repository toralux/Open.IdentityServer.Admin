// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityResource
{
    public class IdentityResourcePropertyRequestedEvent : AuditEvent
    {
        public IdentityResourcePropertiesDto IdentityResourceProperties { get; set; }

        public IdentityResourcePropertyRequestedEvent(IdentityResourcePropertiesDto identityResourceProperties)
        {
            IdentityResourceProperties = AuditEventDataSanitizer.Sanitize(identityResourceProperties);
        }
    }
}
