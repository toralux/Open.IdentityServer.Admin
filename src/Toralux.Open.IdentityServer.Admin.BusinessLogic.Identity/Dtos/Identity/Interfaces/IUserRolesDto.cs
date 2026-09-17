// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared.Dtos.Common;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IUserRolesDto : IBaseUserRolesDto
    {
        string UserName { get; set; }
        List<SelectItemDto> RolesList { get; set; }
        List<IRoleDto> Roles { get; }
        int PageSize { get; set; }
        int TotalCount { get; set; }
    }
}
