// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiSecretsRequestedEvent : AuditEvent
    {
        public ApiSecretsDto ApiSecrets { get; set; }

        public ApiSecretsRequestedEvent(ApiSecretsDto apiSecrets)
        {
            ApiSecrets = AuditEventDataSanitizer.Sanitize(apiSecrets);
        }
    }
}
