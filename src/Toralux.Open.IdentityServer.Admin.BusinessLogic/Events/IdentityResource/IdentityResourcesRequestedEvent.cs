// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityResource
{
    public class IdentityResourcesRequestedEvent : AuditEvent
    {
        public IdentityResourcesDto IdentityResources { get; }

        public IdentityResourcesRequestedEvent(IdentityResourcesDto identityResources)
        {
            IdentityResources = identityResources;
        }
    }
}