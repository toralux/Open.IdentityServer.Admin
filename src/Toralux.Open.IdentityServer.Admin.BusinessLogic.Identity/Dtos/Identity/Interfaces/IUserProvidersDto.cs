// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IUserProvidersDto : IUserProviderDto
    {
        List<IUserProviderDto> Providers { get; }
    }
}
