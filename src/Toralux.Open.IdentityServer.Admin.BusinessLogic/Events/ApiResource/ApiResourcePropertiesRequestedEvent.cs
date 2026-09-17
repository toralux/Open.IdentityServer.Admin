// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourcePropertiesRequestedEvent : AuditEvent
    {
        public ApiResourcePropertiesRequestedEvent(int apiResourceId, ApiResourcePropertiesDto apiResourceProperties)
        {
            ApiResourceId = apiResourceId;
            ApiResourceProperties = AuditEventDataSanitizer.Sanitize(apiResourceProperties);
        }

        public int ApiResourceId { get; set; }
        public ApiResourcePropertiesDto ApiResourceProperties { get; }
    }
}
