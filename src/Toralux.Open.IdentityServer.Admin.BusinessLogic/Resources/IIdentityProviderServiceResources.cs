// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Resources
{
    public interface IIdentityProviderServiceResources
    {
        ResourceMessage IdentityProviderDoesNotExist();

        ResourceMessage IdentityProviderExistsKey();

        ResourceMessage IdentityProviderExistsValue();

    }
}
