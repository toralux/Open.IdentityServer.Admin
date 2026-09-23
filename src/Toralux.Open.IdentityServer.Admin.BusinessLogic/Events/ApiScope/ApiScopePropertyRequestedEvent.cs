// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.ApiScope
{
    public class ApiScopePropertyRequestedEvent : AuditEvent
    {
        public ApiScopePropertyRequestedEvent(int apiScopePropertyId, ApiScopePropertiesDto apiScopeProperty)
        {
            ApiScopePropertyId = apiScopePropertyId;
            ApiScopeProperty = AuditEventDataSanitizer.Sanitize(apiScopeProperty);
        }

        public int ApiScopePropertyId { get; set; }

        public ApiScopePropertiesDto ApiScopeProperty { get; set; }
    }
}
