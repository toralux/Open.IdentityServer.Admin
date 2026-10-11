// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityResource
{
    public class IdentityResourceAddedEvent : AuditEvent
    {
        public IdentityResourceDto IdentityResource { get; set; }

        public IdentityResourceAddedEvent(IdentityResourceDto identityResource)
        {
            IdentityResource = identityResource;
        }
    }
}