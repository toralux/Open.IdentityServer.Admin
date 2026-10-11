// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiSecretRequestedEvent : AuditEvent
    {
        public ApiSecretsDto ApiSecret { get; set; }

        public ApiSecretRequestedEvent(ApiSecretsDto apiSecret)
        {
            ApiSecret = AuditEventDataSanitizer.Sanitize(apiSecret);
        }
    }
}
