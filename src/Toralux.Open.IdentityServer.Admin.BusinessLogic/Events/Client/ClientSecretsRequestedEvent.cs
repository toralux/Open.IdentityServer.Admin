// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.Client
{
    public class ClientSecretsRequestedEvent : AuditEvent
    {
        public ClientSecretsDto ClientSecrets { get; set; }

        public ClientSecretsRequestedEvent(ClientSecretsDto clientSecrets)
        {
            ClientSecrets = AuditEventDataSanitizer.Sanitize(clientSecrets);
        }
    }
}
