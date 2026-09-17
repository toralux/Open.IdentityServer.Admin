// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.Client
{
    public class ClientUpdatedEvent: AuditEvent
    {
        public ClientDto OriginalClient { get; set; }
        public ClientDto Client { get; set; }

        public ClientUpdatedEvent(ClientDto originalClient, ClientDto client)
        {
            OriginalClient = AuditEventDataSanitizer.Sanitize(originalClient);
            Client = AuditEventDataSanitizer.Sanitize(client);
        }
    }
}
