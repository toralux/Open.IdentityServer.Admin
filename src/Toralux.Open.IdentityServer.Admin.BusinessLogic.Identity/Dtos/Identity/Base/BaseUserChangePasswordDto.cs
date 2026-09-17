// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseUserChangePasswordDto<TUserId> : IBaseUserChangePasswordDto
    {
        public TUserId UserId { get; set; }

        object IBaseUserChangePasswordDto.UserId => UserId;
    }
}