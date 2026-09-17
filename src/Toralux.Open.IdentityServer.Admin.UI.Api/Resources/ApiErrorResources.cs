// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Open.IdentityServer.Admin.UI.Api.ExceptionHandling;

namespace Skoruba.Open.IdentityServer.Admin.UI.Api.Resources
{
    public class ApiErrorResources : IApiErrorResources
    {
        public virtual ApiError CannotSetId()
        {
            return new ApiError
            {
                Code = nameof(CannotSetId),
                Description = ApiErrorResource.CannotSetId
            };
        }

        public virtual ApiError IdRequiredForUpdate()
        {
            return new ApiError
            {
                Code = nameof(IdRequiredForUpdate),
                Description = ApiErrorResource.IdRequiredForUpdate
            };
        }
    }
}
