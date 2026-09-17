// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Helpers;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.Client
{
    public class ClientRequestedEvent : AuditEvent
    {
        public ClientDto ClientDto { get; set; }

        public ClientRequestedEvent(ClientDto clientDto)
        {
            ClientDto = AuditEventDataSanitizer.Sanitize(clientDto);
        }
    }
}
