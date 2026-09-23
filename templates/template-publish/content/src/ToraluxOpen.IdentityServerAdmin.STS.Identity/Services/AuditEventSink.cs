// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading;
using System.Threading.Tasks;
using Open.IdentityServer.Events;
using Open.IdentityServer.Services;
using Microsoft.Extensions.Logging;

namespace ToraluxOpen.IdentityServerAdmin.STS.Identity.Services
{
    public class AuditEventSink : DefaultEventSink
    {
        public AuditEventSink(ILogger<DefaultEventService> logger) : base(logger)
        {
        }

        public override Task PersistAsync(Event evt, CancellationToken cancellationToken = default)
        {
            return base.PersistAsync(evt, cancellationToken);
        }
    }
}
