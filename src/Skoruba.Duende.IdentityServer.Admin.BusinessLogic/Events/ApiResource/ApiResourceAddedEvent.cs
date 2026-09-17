// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourceAddedEvent : AuditEvent
    {
        public ApiResourceDto ApiResource { get; set; }

        public ApiResourceAddedEvent(ApiResourceDto apiResource)
        {
            ApiResource = apiResource;
        }
    }
}