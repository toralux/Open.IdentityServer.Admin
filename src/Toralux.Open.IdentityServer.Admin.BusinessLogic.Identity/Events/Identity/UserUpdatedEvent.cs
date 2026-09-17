// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Helpers;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserUpdatedEvent<TUserDto> : AuditEvent
    {
        public TUserDto OriginalUser { get; set; }
        public TUserDto User { get; set; }

        public UserUpdatedEvent(TUserDto originalUser, TUserDto user)
        {
            OriginalUser = AuditEventDataSanitizer.SanitizeUser(originalUser);
            User = AuditEventDataSanitizer.SanitizeUser(user);
        }
    }
}
