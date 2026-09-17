// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization
{
    public interface IIdentityUserAuditSanitizer<TUserDto>
    {
        void SanitizeAuditUser(TUserDto user);
    }
}
