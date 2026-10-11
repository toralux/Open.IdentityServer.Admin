// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Toralux.Open.IdentityServer.Admin.UI.Api.ExceptionHandling;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Resources
{
    public interface IApiErrorResources
    {
        ApiError CannotSetId();
        ApiError IdRequiredForUpdate();
    }
}
