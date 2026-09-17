// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.Client
{
    public class ClientSecretRequestedEvent : AuditEvent
    {
        public ClientSecretsDto ClientSecret { get; set; }

        public ClientSecretRequestedEvent(ClientSecretsDto clientSecret)
        {
            ClientSecret = AuditEventDataSanitizer.Sanitize(clientSecret);
        }
    }
}
