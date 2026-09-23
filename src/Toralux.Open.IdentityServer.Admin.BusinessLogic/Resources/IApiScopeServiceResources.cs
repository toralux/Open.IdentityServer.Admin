// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Resources
{
    public interface IApiScopeServiceResources
    {
        ResourceMessage ApiScopeDoesNotExist();
        ResourceMessage ApiScopeExistsValue();
        ResourceMessage ApiScopeExistsKey();
        ResourceMessage ApiScopePropertyExistsValue();
        ResourceMessage ApiScopePropertyDoesNotExist();
        ResourceMessage ApiScopePropertyExistsKey();
    }
}