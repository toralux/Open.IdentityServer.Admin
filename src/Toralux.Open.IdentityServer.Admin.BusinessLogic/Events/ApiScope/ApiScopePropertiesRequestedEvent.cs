// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.ApiScope
{
    public class ApiScopePropertiesRequestedEvent : AuditEvent
    {
        public ApiScopePropertiesRequestedEvent(int apiScopeId, ApiScopePropertiesDto apiScopeProperties)
        {
            ApiScopeId = apiScopeId;
            ApiResourceProperties = AuditEventDataSanitizer.Sanitize(apiScopeProperties);
        }

        public int ApiScopeId { get; set; }
        public ApiScopePropertiesDto ApiResourceProperties { get; }
    }
}
