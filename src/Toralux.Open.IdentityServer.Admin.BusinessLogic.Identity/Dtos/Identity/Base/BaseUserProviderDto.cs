// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseUserProviderDto<TUserId> : IBaseUserProviderDto
    {
        public TUserId UserId { get; set; }

        object IBaseUserProviderDto.UserId => UserId;
    }
}