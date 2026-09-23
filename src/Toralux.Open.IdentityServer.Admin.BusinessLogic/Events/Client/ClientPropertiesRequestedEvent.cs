// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.Client
{
    public class ClientPropertiesRequestedEvent : AuditEvent
    {
        public ClientPropertiesDto ClientProperties { get; set; }

        public ClientPropertiesRequestedEvent(ClientPropertiesDto clientProperties)
        {
            ClientProperties = AuditEventDataSanitizer.Sanitize(clientProperties);
        }
    }
}
