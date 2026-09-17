// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.Client
{
    public class ClientClaimDeletedEvent : AuditEvent
    {
        public ClientClaimsDto ClientClaim { get; set; }

        public ClientClaimDeletedEvent(ClientClaimsDto clientClaim)
        {
            ClientClaim = clientClaim;
        }
    }
}